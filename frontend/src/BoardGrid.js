import React from 'react';
import './BoardGrid.css';

const BoardGrid=({ onGridClick, gridTokens }) => {
  const numRows=9;
  const numCols=27;

  return (
    <div className="board-grid">
      <div className="grid">
        {Array.from({ length: numRows }).map((_, row) => (
          Array.from({ length: numCols }).map((_, col) => {
            const buttonKey = `${row}-${col}`;
            const tokens = Array.isArray(gridTokens[buttonKey]) ? gridTokens[buttonKey] : [];

            return (
              <button
                key={buttonKey}
                className="grid-button"
                onClick={() => onGridClick(row, col)} 
              >
                {}
                {tokens.map((token, index) => (
                  <img
                    key={index}
                    src={token.src}
                    alt={`Token ${index}`}
                    className="grid-token-image"
                    style={{
                      transform: `translate(${token.offsetX}px, ${token.offsetY}px)`, 
                    }}
                  />
                ))}
              </button>
            );
          })
        ))}
      </div>
    </div>
  );
};

export default BoardGrid;
