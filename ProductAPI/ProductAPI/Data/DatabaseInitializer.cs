using Dapper;

namespace ProductAPI.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IDbConnectionFactory connectionFactory)
        {
            using var connection = await connectionFactory.CreateConnectionAsync();

            // Create Products table
            var createProductsTable = @"
                CREATE TABLE IF NOT EXISTS products (
                    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                    codigo VARCHAR(50) NOT NULL UNIQUE,
                    descricao VARCHAR(500) NOT NULL,
                    departamento VARCHAR(10) NOT NULL,
                    preco DECIMAL(10,2) NOT NULL,
                    status BOOLEAN NOT NULL DEFAULT true,
                    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
                    is_deleted BOOLEAN NOT NULL DEFAULT false
                );

                CREATE INDEX IF NOT EXISTS idx_products_codigo ON products(codigo);
                CREATE INDEX IF NOT EXISTS idx_products_departamento ON products(departamento);
                CREATE INDEX IF NOT EXISTS idx_products_is_deleted ON products(is_deleted);
                CREATE INDEX IF NOT EXISTS idx_products_status ON products(status);
            ";

            await connection.ExecuteAsync(createProductsTable);

            // Create Departments table and insert initial data
            var createDepartmentsTable = @"
                CREATE TABLE IF NOT EXISTS departments (
                    codigo VARCHAR(10) PRIMARY KEY,
                    descricao VARCHAR(100) NOT NULL
                );

                INSERT INTO departments (codigo, descricao) VALUES 
                    ('010', 'BEBIDAS'),
                    ('020', 'CONGELADOS'),
                    ('030', 'LATICINIOS'),
                    ('040', 'VEGETAIS')
                ON CONFLICT (codigo) DO NOTHING;
            ";

            await connection.ExecuteAsync(createDepartmentsTable);

            // Insert sample products (only if table is empty)
            var productCount = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(*) FROM products WHERE is_deleted = false");

            if (productCount == 0)
            {
                var insertSampleProducts = @"
                    INSERT INTO products (codigo, descricao, departamento, preco, status) VALUES 
                    ('COCA001', 'Coca-Cola 350ml', '010', 4.50, true),
                    ('COCA002', 'Coca-Cola 600ml', '010', 6.90, true),
                    ('PEPSI001', 'Pepsi 350ml', '010', 4.20, true),
                    ('AGUA001', 'Água Mineral 500ml', '010', 2.50, true),
                    ('SUCO001', 'Suco de Laranja 1L', '010', 8.90, false),
                    
                    ('PIZZA001', 'Pizza Margherita Congelada', '020', 15.90, true),
                    ('PIZZA002', 'Pizza Calabresa Congelada', '020', 17.50, true),
                    ('SORVETE001', 'Sorvete Chocolate 2L', '020', 12.90, true),
                    ('HAMBUR001', 'Hambúrguer Bovino Congelado', '020', 22.90, true),
                    
                    ('LEITE001', 'Leite Integral 1L', '030', 5.90, true),
                    ('QUEIJO001', 'Queijo Mussarela 500g', '030', 18.90, true),
                    ('IOGURTE001', 'Iogurte Natural 170g', '030', 3.50, true),
                    ('MANTEIGA001', 'Manteiga com Sal 200g', '030', 8.90, true),
                    
                    ('TOMATE001', 'Tomate kg', '040', 6.90, true),
                    ('ALFACE001', 'Alface Americana unidade', '040', 3.50, true),
                    ('CENOURA001', 'Cenoura kg', '040', 4.90, true),
                    ('BATATA001', 'Batata Inglesa kg', '040', 5.50, true),
                    ('CEBOLA001', 'Cebola kg', '040', 4.20, false);
                ";

                await connection.ExecuteAsync(insertSampleProducts);
            }
        }
    }
}