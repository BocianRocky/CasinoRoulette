import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './Login.css';

const Login= () => {
  const [login, setLogin]=useState('');
  const [password, setPassword]=useState('');
  const [error, setError]=useState('');
  const navigate=useNavigate();

  const handleSubmit=async (e) => {
    e.preventDefault();
    try{
      const response=await axios.post('http://localhost:5231/api/Account/login', {
        login,
        password
      });
      console.log('Login response data:', response.data);
      if (response.data && response.data.result && response.data.result.accessToken) {
        const token = response.data.result.accessToken;
        localStorage.setItem('accessToken', token);
        
        await createNewSpin(token);
        navigate('/account-data');
      }else{
        setError('fail with accesToken');
      }
    }catch(err){
      setError('Login failed');
      console.error('Login error:', err);
    }
  };

  const createNewSpin=async (token) => {
    try{
      
      const spinResponse=await axios.post('http://localhost:5231/api/Spin/spin', null, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      console.log('create new spin:', spinResponse.data);
    }catch(err){
      console.error('Error with spin:', err);
    }
  };

  return (
    <div className="login-background">
      <div className="login-box">
        <h1>User Login</h1>
        <form onSubmit={handleSubmit}>
          <div className="login">
            <input
              type="login"
              value={login}
              onChange={(e) => setLogin(e.target.value)}
              placeholder="Login"
            />
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Password"
            />
          </div>
          <button type="submit">User Login</button>
        </form>
        {error && <p className="error">{error}</p>}
        <button className="register-button" onClick={() => navigate('/register')}>Register</button>
      </div>
    </div>
  );
};

export default Login;
