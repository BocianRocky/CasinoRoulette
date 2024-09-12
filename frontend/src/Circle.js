import React from 'react';
import './Circle.css';

const Circle=({ color, borderColor, isActive, onClick }) => {
  return (
    <div
      className={`circle ${isActive ? 'active' : ''}`}
      style={{ backgroundColor: color, borderColor: isActive ? borderColor : 'transparent' }}
      onClick={onClick}
    ></div>
  );
};

export default Circle;
