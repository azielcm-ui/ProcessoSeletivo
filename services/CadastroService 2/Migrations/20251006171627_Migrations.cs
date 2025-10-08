using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cadastro.API.Migrations
{
    /// <inheritdoc />
    public partial class Migrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_DataInicio",
                table: "Funcionarios",
                column: "DataInicio");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionarios_Setor",
                table: "Funcionarios",
                column: "Setor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_DataInicio",
                table: "Funcionarios");

            migrationBuilder.DropIndex(
                name: "IX_Funcionarios_Setor",
                table: "Funcionarios");
        }
    }
}
