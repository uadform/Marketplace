-- liquibase formatted sql

-- changeset u.adomavicius:1
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    external_user_id INT UNIQUE NOT NULL
);
-- rollback DROP TABLE users;

-- changeset u.adomavicius:2
CREATE TABLE items (
    item_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    seller_id INT NOT NULL,
    FOREIGN KEY (seller_id) REFERENCES users(user_id)
);
-- rollback DROP TABLE items;

-- changeset u.adomavicius:3
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    is_paid BOOLEAN DEFAULT FALSE,
    is_completed BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
-- rollback DROP TABLE orders;

-- changeset u.adomavicius:4
CREATE TABLE order_items (
    order_item_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL,
    item_id INT NOT NULL,
    quantity INT NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (item_id) REFERENCES items(item_id)
);
-- rollback DROP TABLE order_items;

-- changeset u.adomavicius:5
-- Sample data insertion
INSERT INTO users (external_user_id) VALUES (1), (2), (3);
INSERT INTO items (name, price, seller_id) VALUES ('Item 1', 100.00, 1), ('Item 2', 150.00, 2);
-- Note: Actual user IDs should match those from the external system for consistency.

-- rollback DELETE FROM items WHERE name IN ('Item 1', 'Item 2');
-- rollback DELETE FROM users WHERE external_user_id IN (1, 2, 3);
