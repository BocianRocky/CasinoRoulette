using System.Data;
using CasinoRoulette.Context;
using CasinoRoulette.DTO;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace CasinoRoulette.Repositories;

public interface IAdminRepository
{
    Task<List<PlayerProfitDto>> GetAllPlayersAndProfit();
}

public class AdminRepository : IAdminRepository
{
    private readonly MasterContext _context;

    public AdminRepository(MasterContext context)
    {
        _context = context;
    }
    public async Task<List<PlayerProfitDto>> GetAllPlayersAndProfit()
    {
        var query = @"SELECT p.FirstName, p.LastName, p.Email, p.Telephone, p.AccountBalance,
                                 SUM(CASE
                                     WHEN b.Result=1 THEN
                                         CASE
                                             WHEN b.BetType='StraightUp' 
                                                 THEN 36*b.BetAmount
                                             WHEN b.BetType='Split' 
                                                 THEN 18*b.BetAmount
                                             WHEN b.BetType='Street' 
                                                 THEN 12*b.BetAmount
                                             WHEN b.BetType='Corner' 
                                                 THEN 9*b.BetAmount
                                             WHEN b.BetType='Line' 
                                                 THEN 6*b.BetAmount
                                             WHEN b.BetType='Dozen' 
                                                 THEN 3*b.BetAmount
                                             WHEN b.BetType='Column' 
                                                 THEN 3*b.BetAmount
                                             WHEN b.BetType IN('Even','Odd','Red','Black','Low','High') THEN 2*b.BetAmount
                                             ELSE 0
                                         END
                                     ELSE 0
                                 END)-SUM(b.BetAmount) AS Profit
                          FROM Bet b
                          JOIN Player p ON b.PlayerId = p.PlayerId
                          GROUP BY p.FirstName,p.LastName,p.Email,p.Telephone,p.AccountBalance";
        using (IDbConnection dbConn = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
        {
            dbConn.Open();
            var players = await dbConn.QueryAsync<PlayerProfitDto>(query);
            return players.ToList();
        }

    }
}