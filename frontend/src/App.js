import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './Login';
import Register from './Register';
import AccountData from './AccountData';
import Deposit from './Deposit';

const App = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/account-data" element={<AccountData />} />
        <Route path="/deposit" element={<Deposit />}/>
      </Routes>
    </Router>
  );
};

export default App;
