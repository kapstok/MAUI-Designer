# Randvoorwaarden

## Frameworks

De MAUI Designer moet geschreven worden met behulp van MAUI.
Het heeft een voordeel: je kunt op een [generieke manier](https://learn.microsoft.com/en-us/dotnet/maui/xaml/runtime-load?view=net-maui-8.0) alle XAML files inladen. Op die manier hoef je dus niet zelf te bepalen op welke plek een bepaald element moet staan.

## Cross OS

De MAUI Designer moet voor verschillende besturingssystemen gecompileerd worden.
Het is van belang dat deze repository wordt aangevuld met documentatie over hoe je met Rider
de MAUI Designer kunt compileren voor een andere OS dan je huidige.

## Eenvoud

Het moet eenvoudig zijn om de MAUI Designer te installeren en gebruiken.
Het zou van toegevoegde waarde zijn als er ook een installer wordt toegevoegd aan
deze repository om de MAUI Designer te kunnen installeren.

Wanneer is het installeren en gebruik eenvoudig genoeg? Een eerstejaarsstudent HBO-ICT
die nog maar een paar colleges Software Engineering 1 heeft gevolgd, moet zelfstandig
de MAUI Designer kunnen installeren en gebruiken.

## Hotreload

Als je een wijziging doet in de XML, moet je Live in de Designer de wijziging kunnen zien.

## Security

Met het ontwerpen van de MAUI Designer, moet aan security worden gedacht. Stel, je laadt een XAML file in en die bevat een `Button` met een `click` attribuut naar de functie `Foo`. Toevallig is er in de broncode van de MAUI Designer een functie gedefinieerd met precies dezelfde naam. Dan moet het voor de gebruiker niet mogelijk zijn om die functie aan te roepen door op die `Button` te drukken. Dat zou feitelijk een voorbeeld van [Code Injection](https://owasp.org/www-community/attacks/Code_Injection) zijn.
