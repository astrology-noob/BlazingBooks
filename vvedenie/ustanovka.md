# 🛠 Установка

<details>

<summary>Шаг 1: Склонируйте репозиторий</summary>

Склонируйте репозиторий [astrology-noob/BlazingBooks](https://github.com/astrology-noob/BlazingBooks) с помощью команды `git clone https://github.com/astrology-noob/BlazingBooks` или с помощью Github Desktop.

</details>

<details>

<summary>Шаг 2: Установите все необходимые зависимости</summary>

В проекте должны присутствовать следующие зависимости:

* Microsoft.EntityFrameworkCore
* Microsoft.EntityFrameworkCore.Design
* Microsoft.EntityFrameworkCore.SqlServer
* Microsoft.EntityFrameworkCore.Tools

</details>

<details>

<summary>Шаг 3: Создайте миграцию базы данных</summary>

* Откройте терминал в корне проекта.

<!---->

* Выполните команды&#x20;
  * `dotnet ef migrations add InitialCreate`&#x20;
  * `dotnet ef database update`

</details>

<details>

<summary>Шаг 4: Добавьте свою строку подключения</summary>

В файле appsettings.json замените строку под ключом "AppDbContext" на вашу строку подключения к БД.

</details>

<details>

<summary>Шаг 5: Запустите проект</summary>

В терминале в корне проекта выполните команду `dotnet run`.

</details>
