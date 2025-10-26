reagent-effect-guidebook-modify-arousal =
    { $chance ->
        [1] Modifies
        *[other] modify
    } arousal by [color=pink]{NATURALFIXED($amount, 3)}[/color]%{ $max ->
    
        [-1] ...
        *[other], up to a maximum arousal of [color=pink]{NATURALFIXED($max, 3)}[/color]%
    }
     