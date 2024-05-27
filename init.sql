CREATE TABLE UsersType (
	idType INTEGER NOT NULL PRIMARY KEY,
	description TEXT
);

CREATE TABLE Users (
	idUser INTEGER NOT NULL PRIMARY KEY,
	Username text NOT NULL UNIQUE,
	Password text NOT NULL,
	Telefon NUMERIC,
	Adresa TEXT,
	Type INTEGER DEFAULT 0,
	FOREIGN KEY (Type) REFERENCES UsersType(idType)
);

CREATE TABLE Magazin (
	idMagazin INTEGER NOT NULL PRIMARY KEY,
	Nume TEXT NOT NULL,
	Telefon NUMERIC NOT NULL,
	Adresa TEXT NOT NULL
);

CREATE TABLE Produs (
	idPart INTEGER NOT NULL PRIMARY KEY,
	Nume TEXT NOT NULL,
	Pret NUMERIC NOT NULL,
	Cantitate NUMERIC NOT NULL DEFAULT 20,
	idMagazin INTEGER NOT NULL,
	FOREIGN KEY (idMagazin) REFERENCES Magazin(idMagazin)
);

CREATE TABLE Comenzi (
	idComanda INTEGER NOT NULL PRIMARY KEY,
	Data date NOT NULL,
	Cantitate NUMERIC NOT NULL DEFAULT 1,
	Adresa TEXT,
	idPiese INTEGER NOT NULL,
	FOREIGN KEY (idPiese) REFERENCES Produs(idPart)
);

INSERT INTO UsersType (idType, description) VALUES
(0, 'Utilizator'),
(1, 'Moderator'),
(2, 'Administrator');