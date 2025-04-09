using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrontEnd.Migrations
{
    /// <inheritdoc />
    public partial class IniciarBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clinica",
                columns: table => new
                {
                    id_clinica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    direccion = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sucursal__6CB481891F75FDB9", x => x.id_clinica);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_rol = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__6AB40045C3E61134", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Personal",
                columns: table => new
                {
                    id_personal = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    contrasenna = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    foto_perfil = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    id_rol = table.Column<int>(type: "int", nullable: true),
                    id_clinica = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__63C76BE24D0D9412", x => x.id_personal);
                    table.ForeignKey(
                        name: "FK__Usuario__id_rol__440B1D61",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "id_rol");
                    table.ForeignKey(
                        name: "FK__Usuario__id_sucu__44FF419A",
                        column: x => x.id_clinica,
                        principalTable: "Clinica",
                        principalColumn: "id_clinica");
                });

            migrationBuilder.CreateTable(
                name: "Propietario",
                columns: table => new
                {
                    id_propietario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    correo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    contrasenna = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    telefono = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    id_rol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Cliente__677F38F5F967AE8F", x => x.id_propietario);
                    table.ForeignKey(
                        name: "FK__Cliente__id_rol__412EB0B6",
                        column: x => x.id_rol,
                        principalTable: "Rol",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    id_notificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_personal = table.Column<int>(type: "int", nullable: true),
                    mensaje = table.Column<string>(type: "text", nullable: true),
                    fecha_envio = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    leida = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__8E852C9AE53D794E", x => x.id_notificacion);
                    table.ForeignKey(
                        name: "FK__Notificac__id_us__5165187F",
                        column: x => x.id_personal,
                        principalTable: "Personal",
                        principalColumn: "id_personal");
                });

            migrationBuilder.CreateTable(
                name: "Cita",
                columns: table => new
                {
                    id_cita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_propietario = table.Column<int>(type: "int", nullable: true),
                    id_clinica_origen = table.Column<int>(type: "int", nullable: true),
                    id_clinica_destino = table.Column<int>(type: "int", nullable: true),
                    motivo_consulta = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    peso_mascota = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    detalles_mascota = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    estado_cita = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    fecha_hora_cita = table.Column<DateTime>(type: "datetime", nullable: true),
                    id_veterinario_asignado = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paquete__755269F3A09E1D66", x => x.id_cita);
                    table.ForeignKey(
                        name: "FK__Paquete__id_clie__46E78A0C",
                        column: x => x.id_propietario,
                        principalTable: "Propietario",
                        principalColumn: "id_propietario");
                    table.ForeignKey(
                        name: "FK__Paquete__id_sucu__47DBAE45",
                        column: x => x.id_clinica_origen,
                        principalTable: "Clinica",
                        principalColumn: "id_clinica");
                    table.ForeignKey(
                        name: "FK__Paquete__id_sucu__48CFD27E",
                        column: x => x.id_clinica_destino,
                        principalTable: "Clinica",
                        principalColumn: "id_clinica");
                    table.ForeignKey(
                        name: "FK__Paquete__id_tran__49C3F6B7",
                        column: x => x.id_veterinario_asignado,
                        principalTable: "Personal",
                        principalColumn: "id_personal");
                });

            migrationBuilder.CreateTable(
                name: "CitaPersonalClinica",
                columns: table => new
                {
                    id_cita_personal_clinica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cita = table.Column<int>(type: "int", nullable: true),
                    id_personal = table.Column<int>(type: "int", nullable: true),
                    id_clinica = table.Column<int>(type: "int", nullable: true),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PaqueteU__A63B5610B93E8DD2", x => x.id_cita_personal_clinica);
                    table.ForeignKey(
                        name: "FK__PaqueteUs__id_pa__5441852A",
                        column: x => x.id_cita,
                        principalTable: "Cita",
                        principalColumn: "id_cita");
                    table.ForeignKey(
                        name: "FK__PaqueteUs__id_su__5629CD9C",
                        column: x => x.id_clinica,
                        principalTable: "Clinica",
                        principalColumn: "id_clinica");
                    table.ForeignKey(
                        name: "FK__PaqueteUs__id_us__5535A963",
                        column: x => x.id_personal,
                        principalTable: "Personal",
                        principalColumn: "id_personal");
                });

            migrationBuilder.CreateTable(
                name: "HistorialCita",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cita = table.Column<int>(type: "int", nullable: true),
                    fecha_cambio = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    estado_anterior = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    estado_nuevo = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ubicacion_actual = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    id_personal_modificacion = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Historia__D3D8101E774AF533", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK__Historial__id_pa__4E88ABD4",
                        column: x => x.id_cita,
                        principalTable: "Cita",
                        principalColumn: "id_cita");
                    table.ForeignKey(
                        name: "FK__Historial__id_us__4F7CD00D",
                        column: x => x.id_personal_modificacion,
                        principalTable: "Personal",
                        principalColumn: "id_personal");
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cita = table.Column<int>(type: "int", nullable: true),
                    monto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    fecha_pago = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    metodo_pago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Pago__C5F54C5AE6E038D0", x => x.id_pago);
                    table.ForeignKey(
                        name: "FK__Pago__id_paquete__4CA06362",
                        column: x => x.id_cita,
                        principalTable: "Cita",
                        principalColumn: "id_cita");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cita_id_clinica_destino",
                table: "Cita",
                column: "id_clinica_destino");

            migrationBuilder.CreateIndex(
                name: "IX_Cita_id_clinica_origen",
                table: "Cita",
                column: "id_clinica_origen");

            migrationBuilder.CreateIndex(
                name: "IX_Cita_id_propietario",
                table: "Cita",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "IX_Cita_id_veterinario_asignado",
                table: "Cita",
                column: "id_veterinario_asignado");

            migrationBuilder.CreateIndex(
                name: "IX_CitaPersonalClinica_id_cita",
                table: "CitaPersonalClinica",
                column: "id_cita");

            migrationBuilder.CreateIndex(
                name: "IX_CitaPersonalClinica_id_clinica",
                table: "CitaPersonalClinica",
                column: "id_clinica");

            migrationBuilder.CreateIndex(
                name: "IX_CitaPersonalClinica_id_personal",
                table: "CitaPersonalClinica",
                column: "id_personal");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCita_id_cita",
                table: "HistorialCita",
                column: "id_cita");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCita_id_personal_modificacion",
                table: "HistorialCita",
                column: "id_personal_modificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacion_id_personal",
                table: "Notificacion",
                column: "id_personal");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_id_cita",
                table: "Pago",
                column: "id_cita");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_id_clinica",
                table: "Personal",
                column: "id_clinica");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_id_rol",
                table: "Personal",
                column: "id_rol");

            migrationBuilder.CreateIndex(
                name: "IX_Propietario_id_rol",
                table: "Propietario",
                column: "id_rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitaPersonalClinica");

            migrationBuilder.DropTable(
                name: "HistorialCita");

            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "Cita");

            migrationBuilder.DropTable(
                name: "Propietario");

            migrationBuilder.DropTable(
                name: "Personal");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "Clinica");
        }
    }
}
