namespace CasinoRoulette.Models;

public enum TypeOfBet
{
    Odd=1,Even=1,Red=1,Black=1,Low=1,High=1,    //(1:1)
    Dozen=2,Column=2,                           //(2:1)
    SixLine=5,                                  //(5:1)
    Corner=8,                                   //(8:1)
    Split= 11,                                  //(11:1) 
    Street=17,                                  //(17:1)
    StraightUp=35                               //(35:1)
}

