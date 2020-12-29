CREATE TABLE "user" (
    id INT GENERATED ALWAYS AS IDENTITY,
    email VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    is_email_confirmed BOOLEAN NOT NULL DEFAULT false,
    PRIMARY KEY(id)
);

CREATE TABLE "date_item" (
    id INT GENERATED ALWAYS AS IDENTITY,
    user_id INT NOT NULL,
    title VARCHAR(255) NOT NULL,
    date DATE NOT NULL,
    PRIMARY KEY(id),
    CONSTRAINT fk_user
        FOREIGN KEY(user_id)
            REFERENCES "user"(id)
            ON DELETE CASCADE
);


CREATE TABLE "cover_item"(
    id INT GENERATED ALWAYS AS IDENTITY,
    user_id INT NOT NULL UNIQUE,
    date_item_id INT NOT NULL UNIQUE,
    PRIMARY KEY(id),
    CONSTRAINT fk_user
        FOREIGN KEY(user_id)
            REFERENCES "user"(id) 
            ON DELETE CASCADE,
    CONSTRAINT fk_date_item
        FOREIGN KEY(date_item_id)
            REFERENCES "date_item"(id) 
            ON DELETE CASCADE
);