
# Pete the shadow racer
### Feruz Fazilov, Karim Ibrahim, Henry Adjei 2ITSOF2

In deze VR game zult u (Pete) racen tegen een ML-agent. Dit document zal u leren hoe u deze ml-agent zelf kunt maken.

## Methoden
### Installatie
- Unity hub met editor versie 2022.3.19f1
  - Te installeren Unity Packages binnen uw URP 3D project
  - ![Unity Packages](/Images/packages.png)
- Anaconda met Python versie 3.9.18 environment 
  - torch~=1.7.1
  - mlagents==0.30.0
  - protobuf==3.20.*

### Waarom AI?
We zullen AI gebruiken, omdat dit de speler zal uitdagen om betere strategieën toe te passen, waardoor de spelervaring meer uitdagend wordt.  We kunnen ook verschillende moeilijkheidsgraden aanbieden aan spelers, dit zorgt voor een hoge herspeelbaarheid omdat elke spelsessie uniek en uitdagend. De agent past zich aan aan de moeilijkheidsgraad die de speler heeft gekozen. 

### Waarom VR?
Virtual reality biedt een intuïtieve ervaring door natuurlijke interacties te simuleren en het biedt ook een immersive ervaring door een gevoel van aanwezigheid te creëren.

### Verloop van het spel
Wanneer u het spel opstart zult u een startscherm zien, deze zal u een keuze geven tussen "play" of "exit", "play" brengt u naar de "Race"-scene waar u meteen kunt beginnen te racen tegen de ML-Agent. Dit doet u door uw controllers te gebruiken om het stuur vast te pakken met de grip knoppen en op de triggers te drukken om vooruit te gaan. De eerste die 3 rondes aflegt wint. Eens gewonnen/verloren krijgt u de keuze opnieuw te beginnen of de game te sluiten. 

Waar we afweken in vergelijken met de one-pager is dat we geen knop gebruiken in de auto om de speler sneller te maken, in de plaats daarvan gebruiken we plekken op de koers (speed-pads) waar, eens over gereden u versnelt voor een paar seconden. Dit komt omdat het maken van de speed-pads veel simpeler is en toch hetzelfde resultaat geeft. Drukken op de knop en sturen tegelijkertijd zorgt ook voor te veel onnodige complexiteit. We hebben ook geen moeilijkheidsgraad toegepast na het drukken op de start-knop kom je direct op de enige "race"-scene terrecht.

### Observaties & Acties
De agent verzamelt observaties uit de omgeving zoals de positie van checkpoints en de afstand tot de checkpoint. Deze verzameling van informaties wordt gebruikt om de volgende actie van een agent te bepalen. De ray perception van de agent een muur detecteert, zal de agent een lichte beweging doen om de collision te ontwijken.
#### Checkpoints
![Checkpoints](/Images/checkpoints.png)
#### Walls
![Walls](/Images/walls.png)
#### Ray-perception
![Rays](/Images/rays.png)

### Beloningen
Om de training van een agent tot een goed einde te laten brengen hebben we gebruik gemaakt van een beloning systeem met penalty’s en beloningen.
Penalty’s
- **Bewegingspenalty**: Wanneer de agent beweegt wordt een zeer kleine penalty gedaan dit zorgt ervoor dat de agent sneller het parkour aflegt en geen onnodige bewegingen doe.

- **Collisionspenalty**: De object “border” heeft kinder objecten “wall” dit is wat de agent moet vermijden anders wordt er een grote penalty toegekend.

- **Checkpoint beloning**: Wanneer de agent een checkpoint bereikt wordt er een positieve beloning gegeven waardoor de agent wordt gemotiveerd om zich beter te navigeren.

## Beschrijving Objecten
### Scenes
- Start
- Race
- Lose
- Finish

### Start Scene
Objects
- XR Interaction Manager – XRInteractionManagerScript
- XR Rig
- User Interface – interface om onze canvas te laten zien, heeft text en twee

Buttons
- Play Button – Play script when clicked loads Race Scene
- Exit Button – Exit script when clicked exits game
- Environment – environment dat de speler in staat.

### Race Scene Objects
- Kart and VR Player: Dit is onze player, dit heeft een XR rig en een auto die bestuurd wordt door de speler
- Kart: Dit is Pete the shadow racer, hij is de AI de je tegen zal racen
- Track: de baan die je op zal racen
- Border: de muur van de baan ( helpt de ai om de baan te volgen door de muren te detecteren)
- Checkpoints: checkpoints dat de Ai moet volgen, 1 van de checkpoints is al finishline gemaakt die ervoor zorgt dat de Lap counter optelt door de script Checkpoint Detector te gebruiken (die roept de LapManager.incrementLap en LapManager.incrementLapAI op)
- SpeedPads: hebben script om karts sneller te laten gaan wanneer je erover rijd
- Canvas: heeft 2 texten, Laps en AiLaps, die tonen de laps aan van de speler en de AI heeft ook de script LapCounter om de texten te updaten
- LapManager: heeft de script LapManager, die zorgt voor alle logica van laps optellen enzovoort. Laadt ook de scene Finish als de speler 3 laps eerst af heeft en laadt de scene Lose als de AI eerst 3 laps af heeft

### Lose and finish scene
- Zelfde als Start scene gewoon andere tekst
## Beschrijvingen van de gedraginen van de objecten


## Resultaten

### 1. Overzicht van de TensorBoard Resultaten
![Cummulative Rewards](/Images/cummulative.png)

In de TensorBoard-omgeving hebben we verschillende runs van getrainde agents geanalyseerd. Deze agents tonen hun resultaten door middel van grafieken zoals de cumulatieve beloningsgrafiek.

### 2. Beschrijving van de Grafiek

#### Runs en Agenten:
- De TensorBoard toont meerdere runs zoals:
  - `Test10\RaceAgent`
  - `Test11\RaceAgent`
  - `test1\KartAgent`
  - etc.

Elke test is gemarkeerd met een unieke kleur.

#### Waarnemingen:
- **Test11\RaceAgent (gele lijn)**: Deze agent toont een stabiele stijging in cumulatieve beloning, wat wijst op dat de agent aan het leren is en aan het verbeteren.
- **Test10\RaceAgent (roze lijn)**: Deze agent laat een stijging zien, maar met wat schommelingen. Dit betekent dat de agent redelijk goed leert.
- **test3\RaceAgent (groene lijn)**: Deze agent heeft scherpe dalingen en schommelingen in de beloning, wat aangeeft dat de training instabiel is of dat de agent slecht leert.

- Andere agenten laten wisselende prestaties zien, met sommige negatieve beloningen of vlakke lijnen, wat wijst op weinig tot geen vooruitgang.

## Bronnen
Valem. (2021, 1 februari). Making my own Mario kart in VR [Video]. YouTube. https://www.youtube.com/watch?v=PRG1PyXPw1g
Unity. (2020, 11 januari). Kart Racing Game with Machine Learning in Unity! (Tutorial) [Video]. YouTube. https://www.youtube.com/watch?v=i0Vt7l3XrIU

Unity-Technologies. (z.d.). ml-agents/docs/Learning-Environment-Examples.md at develop · Unity-Technologies/ml-agents. GitHub. https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Learning-Environment-Examples.md
