INCLUDE Villager.ink
VAR playerName = ""

-> main


=== main ===
This is the main part of the story

Are you curious about the game?
    + [Yes]
        That is super cool!
        -> whatsYourName
    + [No]
        Ewwwwwww get awat from me
        -> END
    
    -> DONE

=== whatsYourName ===
Once i was a adventurer just like you kid...
What is your name?
    * [I do not really remember i have lost my memories]
        ~ playerName = "I do not really remember i have lost my memories"
        WOW THAT IS SUCH A LONG NAME, NICE TO MEET YOU {playerName}
        -> DONE
    * [None of your business .-.]
        okay champion stay calm...
        -> DONE
        

-> END
