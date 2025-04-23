Autorzy: Dominika Bomba i Nikodem Jokiel

Konsolowa aplikacja restauracyjna stworzona w C# z użyciem biblioteki Spectre.Console. Umożliwia zarządzanie użytkownikami, zamówieniami, statusem zamówień oraz generowanie podsumowań przychodów.

Funkcje

Rejestracja i logowanie użytkowników z rolami: Admin, Klient, Kelner, Kucharz
Składanie, gotowanie, podawanie i opłacanie zamówień
Wyświetlanie powiadomień dla kelnera i kucharza
Tabela przychodów i wykres zamówień
Obsługa wielu statusów zamówień (Placed, Cooked, Served, Paid)
Jak uruchomić

Otwórz projekt w Visual Studio
Upewnij się, że masz zainstalowany .NET 8.0
Uruchom projekt (F5 lub Ctrl+F5)
Struktura projektu

Models – klasy danych (np. Order, User, OrderManager)
Services – logika zarządzania, np. UserManager, LogManager
Program.cs – punkt wejścia aplikacji
Biblioteki

Spectre.Console – estetyczne tabele, wykresy i kolory w konsoli
