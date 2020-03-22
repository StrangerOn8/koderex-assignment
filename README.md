
# Introduction 
The code in this solution is provided by `Justin Darmanovich` to show technical skills to `Travis` of **Koderex**.

## Information Sources

I used the below sources to ensure I was using the correct country alpha codes, and to make sure I had the correct widely available coin denominations. 

|                |Country Data                   							|Currency Data													|
|----------------|----------------------------------------------------------|---------------------------------------------------------------|
|United States   |[RestCountries](https://restcountries.eu/rest/v2/alpha/us)|[Wikipedia](https://en.wikipedia.org/wiki/United_States_dollar)|
|United Kingdom  |[RestCountries](https://restcountries.eu/rest/v2/alpha/gb)|[RestCountries](http://projectbritain.com/money.html)			|

## Request Examples

|  |Positive                 															   |Negative																				|Negative Reason											|
|--|---------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------|-----------------------------------------------------------|
|1 |`{ "alpha2Code": "us", "position": { "row": "a", "column": 1 }, "tenderAmount": 1.11 }`|`{ "alpha2Code": "zar", "position": { "row": "a", "column": 1 }, "tenderAmount": 1.10 }`|This will fail, because the alpha code is not supported	|
|2 |`{ "alpha2Code": "gb", "position": { "row": "a", "column": 1 }, "tenderAmount": 8.10 }`|`{ "alpha2Code": "us", "position": { "row": "x", "column": 1 }, "tenderAmount": 1.10 }` |This will fail, because the row doesn't exist				|
|3 |`{ "alpha2Code": "gb", "position": { "row": "b", "column": 1 }, "tenderAmount": 5.10 }`|`{ "alpha2Code": "us", "position": { "row": "a", "column": 1 }, "tenderAmount": 0.01 }` |This will fail, because the tender amount is not enough	|
|4 |`{ "alpha2Code": "gb", "position": { "row": "d", "column": 3 }, "tenderAmount": 7.75 }`|`{ "alpha2Code": "us", "position": { "row": "a", "column": 6 }, "tenderAmount": 0.01 }` |This will fail, because the column doesn't exist			|