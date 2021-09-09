### Stock Portfolio API

## Description
The lazy coder has been trying to build an API to fetch portfolios, get the total value of each portfolio, and delete them. Some parts of the system work but it seems a bit buggy. 

One suspicion is that the currency conversions are at fault but since the lazy developer has moved on to become a gardener no one knows how to identify the problem and potentially fix it; that's where you come in.

## Requirements
The code smells. It is filled with bad decisions, incomplete logic and half baked projects. Your job is, to the best of your ability, refactor the code so that it meets the following criteria:
- An endpoint exists that returns the specified portfolio.
- An endpoint exists that returns the total amount of all stocks in the portfolio in the specified currency.
- An endpoint exists to soft-delete portfolios (whatever that means, we don't know but we don't want to permanently lose the data).
- Has some unit tests covering the logic for all 3 endpoints in the API (note: does not need to be exhaustive).
- Exchange rates should be fetched from CurrencyLayer and we will give you an API key privately. The documentation is available here: https://currencylayer.com/documentation. The account only has permission to get USD exchange rates, so you will need to convert from the base exchange rate to USD and then from USD to the target exchange rate.
- **Apart from the criterias mentioned above, the code test is sent to us by creating a pull request to the main branch.**

As part from these criteria we also want you to refactor the codebase using any or all of the following design principles: KISS, SOLID, YAGNI, DRY, Clean Code, etc. as well as follow best practices when it comes to .net / asp.net / c#. We have no idea what all this means, some consultant just told us. Feel free to use any 3rd party libraries to help you with the task, given they are OSS. Please briefly explain all design decisions and assumptions.

_NOTE: The project StockService is impeccable and is off limits, you cannot change it whatsoever._

## Database
The project uses MongoDB as persistence by default. You are of course free to use another DB but if you do, you will have to explain why, as with any decision made in the code base during a technical interview.

## Database Setup
If you have not used MongoDB before, you can follow these steps for installation and for seeding the database. We optionally recommend using Robo 3T for viewing the data.

# Install MongoDB Community Server: https://www.mongodb.com/try/download/community
# Install MongoDB Database Tools: https://docs.mongodb.com/database-tools/installation/installation-windows/ (install and add to PATH)
# Open a command prompt and browse to /scripts, then run the command "mongoimport --collection=Portfolios --db=portfolioServiceDb portfolios.json" (you should receive confirmation that 2 documents have imported successfully)