-- Criar tabela Funcionarios
CREATE TABLE IF NOT EXISTS "Funcionarios" (
    "Id" UUID PRIMARY KEY,
    "Nome" VARCHAR(200) NOT NULL,
    "Telefone" VARCHAR(20),
    "Email" VARCHAR(200),
    "DataInicio" DATE NOT NULL,
    "Setor" VARCHAR(100),
    "Ativo" BOOLEAN DEFAULT FALSE
);

-- Inserir dados de exemplo
INSERT INTO "Funcionarios" ("Id", "Nome", "Telefone", "Email", "DataInicio", "Setor", "Ativo")
VALUES 
    (gen_random_uuid(), 'João Silva', '1199999999', 'joao.silva@example.com', CURRENT_DATE, 'Financeiro', false),
    (gen_random_uuid(), 'Maria Oliveira', '2198888888', 'maria.oliveira@example.com', CURRENT_DATE + INTERVAL '2 days', 'RH', false),
    (gen_random_uuid(), 'Carlos Souza', '3197777777', 'carlos.souza@example.com', CURRENT_DATE + INTERVAL '5 days', 'TI', false);

-- Identity tables serão criadas via EF Migrations, mas podemos inserir seed direto após criação
-- Usuário admin
DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM "AspNetUsers" WHERE "UserName" = 'admin') THEN
    INSERT INTO "AspNetUsers" ("Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp")
    VALUES (
      gen_random_uuid(),
      'admin',
      'ADMIN',
      'admin@example.com',
      'ADMIN@EXAMPLE.COM',
      true,
      -- Hash da senha "Admin@123" gerado pelo ASP.NET Identity (PBKDF2)
      'AQAAAAIAAYagAAAAECcMw0QjXqKkD3IhGZg2lF0o1w2zayf7XMi9bbCd4s0O3lJhQ0nTQImKpUzK1Cg==',
      gen_random_uuid(),
      gen_random_uuid()
    );
  END IF;
END$$;
