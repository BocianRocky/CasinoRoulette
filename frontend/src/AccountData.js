import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import './AccountData.css';
import BoardGrid from './BoardGrid';
import Tape from './Tape';

import black200 from './tokens/black200.png';
import blue10 from './tokens/blue10.png';
import gray20 from './tokens/gray20.png';
import green5 from './tokens/green5.png';
import red50 from './tokens/red50.png';
import yellow100 from './tokens/yellow100.png';

const AccountData = () => {
  const [accountData, setAccountData]=useState(null);
  const [hotAndColdNumbers, setHotAndColdNumbers]=useState(null);
  const [spinId, setSpinId]=useState(null);
  const [error, setError]=useState('');
  const [selectedImage, setSelectedImage]=useState(null);
  const [selectedChipValue, setSelectedChipValue]=useState(null);
  const [gridTokens, setGridTokens]=useState({});
  const [spinStartTime, setSpinStartTime]=useState(null);
  const [isSpinActive, setIsSpinActive]=useState(false);
  const [remainingTime, setRemainingTime]=useState(0);
  const [showingResult, setShowingResult]=useState(false);
  const [rng, setRng]=useState(null);
  const [infoAboutWon,setInfoAboutWon]=useState(false);
  const [winningAmount,setWinningAmount]=useState(0);

  const navigate=useNavigate();
  


  const fetchData = async () => {
    try{
      const token=localStorage.getItem('accessToken');
      if (!token){
        setError('accessToken not found');
        console.error('accessToken not exists');
        return;
      }

      const accountResponse=await axios.get('http://localhost:5231/api/Account/data', {
        headers:{
          Authorization: `Bearer ${token}`
        }
      });

      setAccountData(accountResponse.data);

      const numbersResponse=await axios.get('http://localhost:5231/api/Spin/nums', {
        headers:{
          Authorization: `Bearer ${token}`
        }
      });

      setHotAndColdNumbers(numbersResponse.data);
    } catch (err){
      setError('Failed');
      console.error('Error:', err);
    }
  };

  const createNewSpin = async () => {
    try{
      const token=localStorage.getItem('accessToken');
      if(!token){
        console.error('accessToken not exists');
        return;
      }
     



      const spinResponse=await axios.post('http://localhost:5231/api/Spin/spin', null, {
        headers:{
          Authorization: `Bearer ${token}`
        }
      });
      setInfoAboutWon(false);
      setGridTokens({});
      setSpinId(spinResponse.data.spinId);
      setSpinStartTime(Date.now());
      setIsSpinActive(true);
      console.log("Generated new spinId:", spinResponse.data.spinId);

      const numbersResponse=await axios.get('http://localhost:5231/api/Spin/nums', {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      setHotAndColdNumbers(numbersResponse.data);

    } catch(err) {
      console.error(err);
    }
  };
  useEffect(() => {
    fetchData();
    createNewSpin(); 
  }, []);



  useEffect(() => {
    if (!isSpinActive) return;

    const updateRemainingTime= () => {
      if (spinStartTime) {
        const elapsedTime=Math.floor((Date.now()-spinStartTime)/1000);
        setRemainingTime(Math.max(0,30-elapsedTime)); 
      }
    };
    updateRemainingTime(); 
    const interval=setInterval(updateRemainingTime, 1000);

    return () => clearInterval(interval);
  }, [spinStartTime, isSpinActive]);

  useEffect(() => {
    if(remainingTime<=0&&isSpinActive) {
      setIsSpinActive(false);
      setShowingResult(true); 
      console.log('Betting time is over');

      const randomNumber=Math.floor(Math.random() * 37);
      console.log('RNG:', randomNumber);
      setRng(randomNumber);

      submitSpinResult(spinId, randomNumber);
      setTimeout(() => {
        (async () => {
          try{
            const token=localStorage.getItem('accessToken');
            const accountResponse=await axios.get('http://localhost:5231/api/Account/data', {
              headers:{
                Authorization: `Bearer ${token}`
              }
            });
            setAccountData(accountResponse.data);
            setInfoAboutWon(true);
          }catch(err){
            console.error(err);
          }
        })();
      }, 6000);

      
      setTimeout(() => {
        setShowingResult(false);
        createNewSpin(); 
        setRemainingTime(30); 
        setSpinStartTime(Date.now()); 
        setIsSpinActive(true); 
      }, 10000); 

      return; 
    }
    
  }, [remainingTime, isSpinActive]);

  const submitSpinResult=async (spinId, numWinner) => {
    try {
      const token=localStorage.getItem('accessToken');
      const resultPayload={
        spinId,
        numWinner,
      };

      const response=await axios.put('http://localhost:5231/api/Bet/results', resultPayload, {
        headers:{
          Authorization: `Bearer ${token}`
        }
      });

      console.log('Winning amount:', response.data); 
      setWinningAmount(response.data.amountWon);
    } catch(err){
      console.error('Error:',err);
    }
  };

  const getRandomOffset= () => {
    const maxOffset = 3;
    const offsetX=Math.floor(Math.random()*(maxOffset*2+1))-maxOffset; 
    const offsetY=Math.floor(Math.random()*(maxOffset*2+1))-maxOffset; 
    return {offsetX, offsetY};
  };

  const handleGridClick=async (row, col) => {
    if(!isSpinActive){
      console.error('Spin is is inactive');
      return;
    }
  
    if(remainingTime<=0){
      console.error('Betting time has expired');
      return;
    }
  
    console.log(`Grid clicked at row ${row}, col ${col}`);
  
    if (!selectedChipValue|| !spinId) {
      console.error('Select a chip or spinId isnt exists');
      return;
    }
  
    const buttonKey=`${row}-${col}`;
    
    
    const gridMapping={
      '0-0': { numbers: [0], type: "StraightUp" },
      '0-1': { numbers: [0, 3], type: "Street" },
      '0-2': { numbers: [3], type: "StraightUp" },
      '0-3': { numbers: [3, 6], type: "Street" },
      '0-4': { numbers: [6], type: "StraightUp" },
      '0-5': { numbers: [6, 9], type: "Street" },

      '0-6': {numbers: [9],type: "StraightUp"},
    '0-7': {numbers: [9,12],type: "Street"},
    '0-8': {numbers: [12],type:"StraightUp"},
    '0-9': {numbers: [12,15],type: "Street"},
    '0-10': {numbers: [15],type:"StraightUp"},
    '0-11': {numbers: [15,18],type:"Street"},
    '0-12': {numbers: [18],type: "StraightUp"},
    '0-13': {numbers: [18,21],type: "Street"},
    '0-14': {numbers: [21],type:"StraightUp"},
    '0-15': {numbers: [21,24],type:"Street"},
    '0-16': {numbers: [24],type:"StraightUp"},
    '0-17': {numbers:[24,27],type:"Street"},
    '0-18': {numbers:[27],type:"StraightUp"},
    '0-19': {numbers:[27,30],type: "Street"},
    '0-20': {numbers:[30],type: "StraightUp"},
    '0-21': {numbers:[30,33],type:"Street"},
    '0-22': {numbers:[33],type:"StraightUp"},
    '0-23': {numbers:[33,36],type:"Street"},
    '0-24': {numbers:[36],type:"StraightUp"},
    '0-26': {numbers:[3,6,9,12,15,18,21,24,27,30,33,36],type:"Dozen"},


    '1-0': {numbers:[0],type:"StraightUp"},
    '1-1': {numbers:[0,2,3],type:"Split"},
    '1-2': {numbers:[2,3],type:"Street"},
    '1-3': {numbers:[2,3,5,6],type:"Corner"},
    '1-4': {numbers:[5,6],type:"Street"},
    '1-5': {numbers:[5,6,8,9],type:"Corner"},
    '1-6': {numbers:[8,9],type:"Street"},
    '1-7': {numbers:[8,9,11,12],type:"Corner"},
    '1-8': {numbers:[11,12],type:"Street"},
    '1-9': {numbers:[11,12,14,15],type:"Corner"},
    '1-10': {numbers:[14,15],type:"Street"},
    '1-11': {numbers:[14,15,17,18],type:"Corner"},
    '1-12': {numbers:[17,18],type:"Street"},
    '1-13': {numbers:[17,18,20,21],type:"Corner"},
    '1-14': {numbers:[20,21],type:"Street"},
    '1-15': {numbers:[20,21,23,24],type:"Corner"},
    '1-16': {numbers:[23,24],type:"Street"},
    '1-17': {numbers:[23,24,26,27],type:"Corner"},
    '1-18': {numbers:[26,27],type:"Street"},
    '1-19': {numbers:[26,27,29,30],type:"Corner"},
    '1-20': {numbers:[29,30],type:"Street"},
    '1-21': {numbers:[29,30,32,33],type:"Corner"},
    '1-22': {numbers:[32,33],type:"Street"},
    '1-23': {numbers:[32,33,35,36],type:"Corner"},
    '1-24': {numbers:[35,36],type:"Street"},
      
    '2-0': {numbers:[0],type:"StraightUp"},
    '2-1': {numbers:[0,2],type:"Street"},
    '2-2': {numbers:[2],type:"StraightUp"},
    '2-3': {numbers:[2,5],type:"Street"},
    '2-4': {numbers:[5],type:"StraightUp"},
    '2-5': {numbers:[5,8],type:"Street"},
    '2-6': {numbers:[8],type:"StraightUp"},
    '2-7': {numbers:[8,11],type:"Street"},
    '2-8': {numbers:[11],type:"StraightUp"},
    '2-9': {numbers: [11,14],type:"Street"},
    '2-10': {numbers: [14],type:"StraightUp"},
    '2-11': {numbers: [14,17],type:"Street"},
    '2-12': {numbers: [17],type:"StraightUp"},
    '2-13': {numbers: [17,20],type:"Street"},
    '2-14': {numbers: [20],type:"StraightUp"},
    '2-15': {numbers: [20,23],type:"Street"},
    '2-16': {numbers: [23],type:"StraightUp"},
    '2-17': {numbers: [23,26],type:"Street"},
    '2-18': {numbers: [26],type:"StraightUp"},
    '2-19': {numbers: [26,29],type:"Street"},
    '2-20': {numbers: [29],type:"StraightUp"},
    '2-21': {numbers: [29,32],type:"Street"},
    '2-22': {numbers: [32],type:"StraightUp"},
    '2-23': {numbers: [32,35],type:"Street"},
    '2-24': {numbers: [35],type:"StraightUp"},
    '2-26': {numbers: [2,5,8,11,14,17,20,23,26,29,32,35],type:"Dozen"},

    '3-0': {numbers:[0],type:"StraightUp"},
    '3-1': {numbers:[0,1,2],type:"Split"},
    '3-2': {numbers:[1,2],type:"Street"},
    '3-3': {numbers:[1,2,4,5],type:"Corner"},
    '3-4': {numbers:[4,5],type:"Street"},
    '3-5': {numbers:[4,5,7,8],type:"Corner"},
    '3-6': {numbers:[7,8],type:"Street"},
    '3-7': {numbers:[7,8,10,11],type:"Corner"},
    '3-8': {numbers:[10,11],type:"Street"},
    '3-9': {numbers:[10,11,13,14],type:"Corner"},
    '3-10': {numbers:[13,14],type:"Street"},
    '3-11': {numbers:[13,14,16,17],type:"Corner"},
    '3-12': {numbers:[16,17],type:"Street"},
    '3-13': {numbers:[16,17,19,20],type:"Corner"},
    '3-14': {numbers:[19,20],type:"Street"},
    '3-15': {numbers:[19,20,22,23],type:"Corner"},
    '3-16': {numbers:[22,23],type:"Street"},
    '3-17': {numbers:[22,23,25,26],type:"Corner"},
    '3-18': {numbers:[25,26],type:"Street"},
    '3-19': {numbers:[25,26,28,29],type:"Corner"},
    '3-20': {numbers:[28,29],type:"Street"},
    '3-21': {numbers:[28,29,31,32],type:"Corner"},
    '3-22': {numbers:[31,32],type:"Street"},
    '3-23': {numbers:[31,32,34,35],type:"Corner"},
    '3-24': {numbers:[34,35],type:"Street"},

    '4-0': {numbers:[0],type:"StraightUp"},
    '4-1': {numbers:[0,1],type:"Street"},
    '4-2': {numbers:[1],type:"StraightUp"},
    '4-3': {numbers:[1,4],type:"Street"},
    '4-4': {numbers:[4],type:"StraightUp"},
    '4-5': {numbers:[4,7],type:"Street"},
    '4-6': {numbers:[7],type:"StraightUp"},
    '4-7': {numbers:[7,10],type:"Street"},
    '4-8': {numbers:[10],type:"StraightUp"},
    '4-9': {numbers:[10,13],type:"Street"},
    '4-10': {numbers:[13],type:"StraightUp"},
    '4-11': {numbers:[13,16],type:"Street"},
    '4-12': {numbers:[16],type:"StraightUp"},
    '4-13': {numbers:[16,19],type:"Street"},
    '4-14': {numbers:[19],type:"StraightUp"},
    '4-15': {numbers:[19,22],type:"Street"},
    '4-16': {numbers:[22],type:"StraightUp"},
    '4-17': {numbers:[22,25],type:"Street"},
    '4-18': {numbers:[25],type:"StraightUp"},
    '4-19': {numbers:[25,28],type:"Street"},
    '4-20': {numbers:[28],type:"StraightUp"},
    '4-21': {numbers:[28,31],type:"Street"},
    '4-22': {numbers:[31],type:"StraightUp"},
    '4-23': {numbers:[31,34],type:"Street"},
    '4-24': {numbers:[34],type:"StraightUp"},
    '4-26': {numbers:[1,4,7,10,13,16,19,22,25,28,31,34],type:"Dozen"},


    '5-2': {numbers:[1,2,3],type:"Split"},
    '5-3': {numbers:[1,2,3,4,5,6],type:"SixLine"},
    '5-4': {numbers:[4,5,6],type:"Split"},
    '5-5': {numbers:[4,5,6,7,8,9],type:"SixLine"},
    '5-6': {numbers:[7,8,9],type:"Split"},
    '5-7': {numbers:[7,8,9,10,11,12],type:"SixLine"},
    '5-8': {numbers:[10,11,12],type:"Split"},
    '5-9': {numbers:[10,11,12,13,14,15],type:"SixLine"},
    '5-10': {numbers:[13,14,15],type:"Split"},
    '5-11': {numbers:[13,14,15,16,17,18],type:"SixLine"},
    '5-12': {numbers:[16,17,18],type:"Split"},
    '5-13': {numbers:[16,17,18,19,20,21],type:"SixLine"},
    '5-14': {numbers:[19,20,21],type:"Split"},
    '5-15': {numbers:[19,20,21,22,23,24],type:"SixLine"},
    '5-16': {numbers:[22,23,24],type:"Split"},
    '5-17': {numbers:[22,23,24,25,26,27],type:"SixLine"},
    '5-18': {numbers:[25,26,27],type:"Split"},
    '5-19': {numbers:[25,26,27,28,29,30],type:"SixLine"},
    '5-20': {numbers:[28,29,30],type:"Split"},
    '5-21': {numbers:[28,29,30,31,32,33],type:"SixLine"},
    '5-22': {numbers:[31,32,33],type:"Split"},
    '5-23': {numbers:[31,32,33,34,35,36],type:"SixLine"},
    '5-24': {numbers:[34,35,36],type:"Split"},

    '6-2': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-3': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-4': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-5': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-6': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-7': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},
    '6-8': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12],type:"Column"},

    '6-10': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-11': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-12': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-13': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-14': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-15': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},
    '6-16': {numbers:[13,14,15,16,17,18,19,20,21,22,23,24],type:"Column"},

    '6-18': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-19': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-20': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-21': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-22': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-23': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},
    '6-24': {numbers:[25,26,27,28,29,30,31,32,33,34,35,36],type:"Column"},

    '7-2': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},
    '7-3': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},
    '7-4': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},
    '8-2': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},
    '8-3': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},
    '8-4': {numbers:[1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18],type:"Low"},

    '7-6': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},
    '7-7': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},
    '7-8': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},
    '8-6': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},
    '8-7': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},
    '8-8': {numbers:[2,4,6,8,10,12,14,16,18,20,22,24,26,28,30,32,34,36],type:"Even"},

    '7-10': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},
    '7-11': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},
    '7-12': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},
    '8-10': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},
    '8-11': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},
    '8-12': {numbers:[1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36],type:"Red"},

    '7-14': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},
    '7-15': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},
    '7-16': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},
    '8-14': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},
    '8-15': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},
    '8-16': {numbers:[2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35],type:"Black"},

    '7-18': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},
    '7-19': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},
    '7-20': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},
    '8-18': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},
    '8-19': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},
    '8-20': {numbers:[1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35],type:"Odd"},

    };
  
  
    const selectedBet=gridMapping[buttonKey];
  
    if(!selectedBet){
      console.error('Invalid bet placement');
      return;
    }
  
    const { numbers, type }=selectedBet;
  
    const { offsetX, offsetY }=getRandomOffset();
  
    setGridTokens((prevGridTokens)=> {
      const existingTokens=Array.isArray(prevGridTokens[buttonKey]) ? prevGridTokens[buttonKey] : [];
      return {
        ...prevGridTokens,
        [buttonKey]: [
          ...existingTokens,
          { src: selectedImage, offsetX, offsetY },
        ],
      };
    });
  
    try{
      const token=localStorage.getItem('accessToken');
      const betPayload={
        spinId,
        gameId: 1,
        betAmount: selectedChipValue,
        betType: type,
        betNumbers: numbers.map(number => ({ number }))
      };
      console.log("Bet data:", betPayload);
      const response=await axios.post('http://localhost:5231/api/Bet/bet', betPayload, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      console.log('Bet', response.data);
      await fetchData();  
    }catch(err){
      console.error('Error:',err);
    }
  };
  

  const handleImageClick=(image, value) => {
    if(selectedImage!==image) {  
      setSelectedImage(image);
      setSelectedChipValue(value); 
      console.log(`Selected chip value: ${value}`); 
    }else{
      setSelectedImage(null);
      setSelectedChipValue(null); 
    }
  };
  const isTableEmpty=Object.keys(gridTokens).length === 0;
  if(error){
    return <div>{error}</div>;
  }

  if(!accountData|| !hotAndColdNumbers){
    return <div>Loading...</div>;
  }

  return (
    <div className="main">
      <div className="top">
        <div className="left-top">
          <div className="data">
            <h2>Account Data</h2>
            <p>User: {accountData.firstName} {accountData.lastName}</p>
            <p>Account Balance: {accountData.accountBalance}</p>
          </div>
          <div className="deposit">
            <button 
              onClick={() => navigate('/deposit')}
              disabled={!isTableEmpty} 
              className={!isTableEmpty ? 'disabled-button' : ''}
            >
              Deposit
            </button>
          </div>
        </div>
        <div className="info-won">
        <p>{infoAboutWon ? (winningAmount > 0 ? `Congratulations! You won: ${winningAmount}`:'Not this time! Try again for a chance to win!'): ''}</p>
        </div>
        <div className="numbers">
          <h2>Hot Numbers</h2>
          <ul>
            {hotAndColdNumbers.hotNumbers.map((number, index) => (
              <li key={index}>{number}</li>
            ))}
          </ul>
  
          <h2>Cold Numbers</h2>
          <ul>
            {hotAndColdNumbers.coldNumbers.map((number, index) => (
              <li key={index}>{number}</li>
            ))}
          </ul>
        </div>
      </div>
      <div className="roulette-table">
        <div className="leftcolumn">
          <div className="table">
            <BoardGrid 
              buttonFunctions={null} 
              onGridClick={handleGridClick} 
              gridTokens={gridTokens}
            />
          </div>
          <div className="timer">
            {showingResult ? (
              <p>NO MORE BETS</p>
            ) : (
              <p>{remainingTime > 0 ? `Czas na obstawienie: ${remainingTime} sekund` : 'NO MORE BETS'}</p>
            )}
          </div>
        </div>
        <div className="roulette">
          <div className="tape">
            <Tape result={rng}/>
          </div>
        </div>
      </div>
      
      <div className="button-container">
        {[{image: green5, value: 5}, {image: blue10, value: 10}, {image: gray20, value: 20}, {image: red50, value: 50}, {image: yellow100, value: 100}, {image: black200, value: 200}].map((chip, index) => (
          <img
            key={index}
            src={chip.image}
            className={`image-button ${selectedImage === chip.image ? 'selected' : ''}`}
            alt={`Token ${index + 1}`}
            onClick={() => handleImageClick(chip.image, chip.value)}
          />
        ))}
      </div>
    </div>
  );
};

export default AccountData;
