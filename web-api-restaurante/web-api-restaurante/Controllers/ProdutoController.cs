﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_api_restaurante.Entidades;

namespace web_api_restaurante.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly string? _connectionString;
        //ctor atalho criar o construtor
        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();    
            return dbConnection;    
        }
        [HttpGet]
        public async Task <IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id,nome,descricao,imagemUrl from Produto; ";
            var result = await dbConnection.QueryAsync<Produto>(sql);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "select id,nome,descricao,imagemUrl from Produto where id = @id; ";

            var produto = await dbConnection.QueryFirstOrDefaultAsync<Produto>(sql, new { id });
            
            dbConnection.Close();
            if(produto == null) 
            return NotFound();  

            return Ok(produto);
            
        }
        [HttpPost]

        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT into Produto(nome,descricao,imagemUrl)
               Values (@Nome,@Descricao, ImagemUrl); commit;";

            await dbConnection.ExecuteAsync(query, produto); 

            return Ok();
        }


    }
}
