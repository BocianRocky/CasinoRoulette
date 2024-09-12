import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './Register.css';

const Register= () => {
  const [formData, setFormData]= useState({
    firstName: '',
    lastName: '',
    email: '',
    login: '',
    telephone: '',
    password: '',
    confirmPassword: '' 
  });

  const [error, setError]=useState('');
  const navigate=useNavigate();

  const handleChange= (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };
  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if(formData.password!==formData.confirmPassword){
      setError('Passwords do not match.');
      return;
    }
    try {
      const { confirmPassword, ...dataToSend } = formData; 
      const response=await axios.post('http://localhost:5231/api/Account/register', dataToSend);
      if(response.status === 200){
        navigate('/');
      }else{
        setError('Registration failed.');
      }
    }catch (err){
      setError('Registration failed. Please check your details.');
      console.error('Registration error:', err);
    }
  };

  return (
    <div className="register-background">
      <div className="register-box">
        <div className="lab-reg">
          <h1>Register</h1>
        </div>
        <div className="register-inputs">
          <form onSubmit={handleSubmit}>
            <input
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              placeholder="First Name"
              required
            />
            <input
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              placeholder="Last Name"
              required
            />
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="Email"
              required
            />
            <input
              type="text"
              name="login"
              value={formData.login}
              onChange={handleChange}
              placeholder="Login"
              required
            />
            <input
              type="text"
              name="telephone"
              value={formData.telephone}
              onChange={handleChange}
              placeholder="Telephone"
              required
            />
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Password"
              required
            />
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="Confirm Password"
              required
            />
            <button type="submit">Register</button>
          </form>
          </div>
        {error && <p>{error}</p>}
        </div>
    </div>
  );
};

export default Register;
