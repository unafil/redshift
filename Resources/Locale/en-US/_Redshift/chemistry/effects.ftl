reagent-effect-guidebook-modify-arousal =
    { $chance ->
        [1] Modifies
        *[other] modify
    } arousal by [color=pink]{NATURALFIXED($amount, 3)}[/color]%{ $positive ->
        [true]{ $max ->
            
            [-1],[color=pink] with no maximum limit[/color]
            *[other], up to a maximum arousal of [color=pink]{NATURALFIXED($max, 3)}[/color]%
        }
        *[false] {""}
    }
