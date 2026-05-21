using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    room_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    area = table.Column<int>(type: "int", nullable: true),
                    max_people = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__rooms__3213E83F675C0AB5", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    service_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__services__3213E83F292F1CED", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    cccd = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__tenants__3213E83FEDA81A33", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F1C887057", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bills",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_id = table.Column<long>(type: "bigint", nullable: true),
                    month = table.Column<int>(type: "int", nullable: true),
                    year = table.Column<int>(type: "int", nullable: true),
                    room_price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    service_total = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bills__3213E83F65FB2837", x => x.id);
                    table.ForeignKey(
                        name: "bills_ibfk_1",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tenant_id = table.Column<long>(type: "bigint", nullable: true),
                    room_id = table.Column<long>(type: "bigint", nullable: true),
                    start_day = table.Column<DateOnly>(type: "date", nullable: true),
                    end_day = table.Column<DateOnly>(type: "date", nullable: true),
                    deposit = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__contract__3213E83F1BC73F54", x => x.id);
                    table.ForeignKey(
                        name: "FK_contracts_rooms",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_contracts_tenants",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "bill_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bill_id = table.Column<long>(type: "bigint", nullable: true),
                    service_id = table.Column<long>(type: "bigint", nullable: true),
                    old_index = table.Column<int>(type: "int", nullable: true),
                    new_index = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    total = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__bill_det__3213E83FBF4D3E3F", x => x.id);
                    table.ForeignKey(
                        name: "bill_details_ibfk_2",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_bill_details_bill",
                        column: x => x.bill_id,
                        principalTable: "bills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bill_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    payment_method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    payment_date = table.Column<DateOnly>(type: "date", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__payments__3213E83F0DCD74CD", x => x.id);
                    table.ForeignKey(
                        name: "payments_ibfk_1",
                        column: x => x.bill_id,
                        principalTable: "bills",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_bill_details_bill_id",
                table: "bill_details",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_bill_details_service_id",
                table: "bill_details",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_bills_room_id",
                table: "bills",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_room_id",
                table: "contracts",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_tenant_id",
                table: "contracts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_bill_id",
                table: "payments",
                column: "bill_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bill_details");

            migrationBuilder.DropTable(
                name: "contracts");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "bills");

            migrationBuilder.DropTable(
                name: "rooms");
        }
    }
}
