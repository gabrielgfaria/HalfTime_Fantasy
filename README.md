**Half-Time Fantasy**

This API simulates a real Fantasy Football application, with subtle differences.
You'll be able to create your account and buy and sell players, as well as edit your Team's name and Country and also your players' Names and Countries.

Once you signup, you'll be assigned a Team automatically. You'll need to append your JWT to the header of every request in order to access most endpoints. To get your JWT simply login using a valid email and password. HAVE FUN!

To start with Half-Time Fantasy, use the below endpoints:

<details><summary>Signup and Login</summary>
<details><summary>POST</summary>

**/api/Authentication/register**


Example Value:

        {
            "email": "user@example.com",
            "password": "string"
        }


**/api/Authentication/login**

Example Value:

        {
            "email": "user@example.com",
            "password": "string"
        }
</details>
</details>

<details><summary>Buy/Sell Players and see Player's market list</summary>
<details><summary>GET</summary>

**/api/Market**

No request body
</details>
<details><summary>POST</summary>

**/api/Market/sell**


Example Value:

        {
            "playerId": 0,
            "value": 2147483647
        }


**/api/Market/buy**

Example Value:

        {
            "playerId": 0
        }
</details>
</details>

<details><summary>See and edit your Players</summary>
<details><summary>GET</summary>

**/api/Player/{playerId}**

No request body
</details>
<details><summary>PATCH</summary>

**/api/Player**


Example Value:

        {
            "playerId": 0,
            "firstName": "string",
            "lastName": "string",
            "country": "string"
        }


</details>
</details>

<details><summary>See and edit your Team</summary>
<details><summary>GET</summary>

**/api/Team**

No request body
</details>
<details><summary>PATCH</summary>

**/api/Team**


Example Value:

        {
            "name": "string",
            "country": "string"
        }


</details>
</details>
</details>







In order to run this solution on your own machine:
1. Use the provided `HalfTime_Fantasy_script.sql` script to generate the Database;
2. Make the necessary changes to the `ConnectionString` on the `appsettings.json` under the `WebUI.csproj`;
3. Come up with a long enough string and add it to the `Secret` key on the `appsettings.json` under the `WebUI.csproj`
