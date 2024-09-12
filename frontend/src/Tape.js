import React, { useEffect, useRef } from 'react';
import './Tape.css'; 

const Tape= ({ result }) => {
  const tapeRef = useRef(null);
  const rouletteNumbers = [35, 3, 26, 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12]; // Twoja niestandardowa lista numerów

  useEffect(() => {
    if(result!==null){
      resetTape(); 
      requestAnimationFrame(() => animateTape(result)); 
    }
  }, [result]);

  const resetTape= () => {
    if(!tapeRef.current) return;

    tapeRef.current.style.transition='none';
    tapeRef.current.style.transform='translateX(0px)';
  };
  const animateTape= (number) => {
    if(!tapeRef.current) return;

    const itemWidth=70; 
    const animationDuration=6000; 
    const extendedNumbers=[...rouletteNumbers, ...rouletteNumbers, ...rouletteNumbers];
    const index=extendedNumbers.findIndex(num => num===number);
    const offset=(index+rouletteNumbers.length*2-3)*itemWidth;
    requestAnimationFrame(() => {
      tapeRef.current.style.transition = `transform ${animationDuration}ms cubic-bezier(0.33, 1, 0.68, 1)`;
      tapeRef.current.style.transform = `translateX(-${offset}px)`;
    });
  };

  const getBackgroundColor = (number) => { 
    if(number===0) return 'green-background';
    if([1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36].includes(number)) return 'red-background';
    return 'black-background';
  };

  return (
    <div className="tape-container">
      <div className="line line-top"></div> {}
      <div className="tape" ref={tapeRef}>
        {Array.from({ length: 111 }, (_, i) => ( //3*37
          <div key={i} className={`tape-item ${getBackgroundColor(rouletteNumbers[i % rouletteNumbers.length])}`}>
            {rouletteNumbers[i % rouletteNumbers.length]}
          </div>
        ))}
      </div>
      <div className="line line-bottom"></div> {}
    </div>
  );
};

export default Tape;
