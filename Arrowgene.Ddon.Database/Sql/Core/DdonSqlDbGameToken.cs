﻿using System.Data.Common;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Sql.Core
{
    public abstract partial class DdonSqlDb<TCon, TCom> : SqlDb<TCon, TCom>
        where TCon : DbConnection
        where TCom : DbCommand
    {
        private const string SqlInsertToken = "INSERT INTO `ddon_game_token` (`account_id`, `character_id`, `token`, `created`) VALUES (@account_id, @character_id, @token, @created);";
        private const string SqlUpdateToken = "UPDATE `ddon_game_token` SET `character_id`=@character_id, `token`=@token, `created`=@created WHERE `account_id` = @account_id;";
        private const string SqlSelectTokenByAccountId = "SELECT `token`, `account_id`, `character_id`, `token`, `created` FROM `ddon_game_token` WHERE `account_id` = @account_id;";
        private const string SqlDeleteTokenByAccountId = "DELETE FROM `ddon_game_token` WHERE `account_id`=@account_id;";
        private const string SqlSelectToken = "SELECT `token`, `account_id`, `character_id`, `token`, `created` FROM `ddon_game_token` WHERE `token` = @token;";
        private const string SqlDeleteToken = "DELETE FROM `ddon_game_token` WHERE `token`=@token;";

        public bool SetToken(GameToken token)
        {
            int rowsAffected = ExecuteNonQuery(SqlUpdateToken, command =>
            {
                AddParameter(command, "@account_id", token.AccountId);
                AddParameter(command, "@character_id", token.CharacterId);
                AddParameter(command, "@token", token.Token);
                AddParameter(command, "@created", token.Created);
            });
            if (rowsAffected > NoRowsAffected)
            {
                return true;
            }

            rowsAffected = ExecuteNonQuery(SqlInsertToken, command =>
            {
                AddParameter(command, "@account_id", token.AccountId);
                AddParameter(command, "@character_id", token.CharacterId);
                AddParameter(command, "@token", token.Token);
                AddParameter(command, "@created", token.Created);
            });

            return rowsAffected > NoRowsAffected;
        }

        public GameToken SelectTokenByAccountId(int accountId)
        {
            GameToken token = null;
            ExecuteReader(SqlSelectTokenByAccountId, command => { AddParameter(command, "@account_id", accountId); }, reader =>
            {
                if (reader.Read())
                {
                    token = new GameToken();
                    token.AccountId = GetInt32(reader, "account_id");
                    token.CharacterId = GetUInt32(reader, "character_id");
                    token.Token = GetString(reader, "token");
                    token.Created = GetDateTime(reader, "created");
                }
            });
            return token;
        }
        
        public GameToken SelectToken(string tokenStr)
        {
            GameToken token = null;
            ExecuteReader(SqlSelectToken, command => { AddParameter(command, "@token", tokenStr); }, reader =>
            {
                if (reader.Read())
                {
                    token = new GameToken();
                    token.AccountId = GetInt32(reader, "account_id");
                    token.CharacterId = GetUInt32(reader, "character_id");
                    token.Token = GetString(reader, "token");
                    token.Created = GetDateTime(reader, "created");
                }
            });
            return token;
        }

        public bool DeleteToken(string token)
        {
            int rowsAffected = ExecuteNonQuery(SqlDeleteToken, command => { AddParameter(command, "@token", token); });
            return rowsAffected > NoRowsAffected;
        }
        
        public bool DeleteTokenByAccountId(int accountId)
        {
            int rowsAffected = ExecuteNonQuery(SqlDeleteTokenByAccountId, command => { AddParameter(command, "@account_id", accountId); });
            return rowsAffected > NoRowsAffected;
        }
    }
}
