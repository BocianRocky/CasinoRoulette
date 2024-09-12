import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from 'axios';
import './Deposit.css';

const Deposit= () => {
    const navigate=useNavigate();
    const [amount, setAmount]=useState(0);
    const [paymentMethod, setPaymentMethod]=useState('');
    const [transactionType, setTransactionType]=useState('');
    const [responseMessage, setResponseMessage]=useState('');

    const handleButtonClick=(value) => {
        setAmount(value);
    }
    
    const handleSubmit=async (e) => {
        e.preventDefault();

        const paymentData={
            amount: parseFloat(amount),
            type: transactionType,
            paymentMethod: paymentMethod
        };

        try{
            const token=localStorage.getItem('accessToken');
            if(!token){
                setResponseMessage('Access token not found.');
                return;
            }

            console.log('Sending Payment Data:', paymentData);
            const response=await axios.post('http://localhost:5231/api/Payment/payment', paymentData, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });
            if(response.status===200){
                setResponseMessage('Payment processed successfully');
            } else {
                setResponseMessage('Payment failed');
            }
        }catch(error){
            console.error('Payment error:', error);
            setResponseMessage('Error');
        }
    }
    const handleMethodChange= (e) => {
        setPaymentMethod(e.target.value);
    }
    const handleTypeChange= (e) => {
        setTransactionType(e.target.value);
    }

    return (
        <div className="payment-background">
            <div className="payment-box">
                <h1>Deposit</h1>
                <div className="elements">
                    <form onSubmit={handleSubmit}>
                        <input 
                            type="text" 
                            value={amount} 
                            onChange={(e) => setAmount(e.target.value)} 
                            placeholder="Enter amount" 
                        />
                        <div className="buttons">
                            <button type="button" onClick={() => handleButtonClick(50)}>50</button>
                            <button type="button" onClick={() => handleButtonClick(100)}>100</button>
                            <button type="button" onClick={() => handleButtonClick(250)}>250</button>
                            <button type="button" onClick={() => handleButtonClick(500)}>500</button>
                            <button type="button" onClick={() => navigate('/account-data')}>Back</button>
                        </div>
                        {}
                        <div>
                            <select id="transaction-type" value={transactionType} onChange={handleTypeChange}>
                                <option value="">-- Select transaction type --</option>
                                <option value="deposit">Deposit</option>
                                <option value="withdrawal">Withdrawal</option>
                                <option value="transfer">Transfer</option>
                                <option value="bonus">Bonus</option>
                            </select>
                        </div>

                        {}
                        <div>
                            <select id="payment-method" value={paymentMethod} onChange={handleMethodChange}>
                                <option value="">-- Select payment method --</option>
                                <option value="credit-card">Credit Card</option>
                                <option value="paypal">PayPal</option>
                                <option value="bank-transfer">Bank Transfer</option>
                                <option value="bitcoin">Bitcoin</option>
                            </select>
                        </div>

                        {}
                        <button type="submit">Submit Payment</button>
                    </form>
                    <div className="response-info">
                        {}
                        {responseMessage && <p>{responseMessage}</p>}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Deposit;
