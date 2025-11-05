# examine inputs: ent, hasUnderwear (bool), arousal (float), uniformType (string, either "suit" or "skirt" or "none"), pastThreshold (bool)
# arousal shouldn't ever really reach 5 for more then a split second since this is always rounded down
# might be worth finding a better way to do this
# also fluent doesn't support empty conditions which pisses me off!!!!!

genital-cock-use-verb = jork it
genital-cock-examine =
    { $uniformType ->
        *[suit] { $pastThreshold ->
            [true] [color=pink]Something is pressing against { POSS-ADJ($ent) } uniform.[/color]
            *[false] {""}
        }
        [skirt] { $hasUnderwear ->
            *[true] {""}
            [false] { $pastThreshold -> 
                [true] [color=pink]There's something poking at { POSS-ADJ($ent) } skirt...[/color]
                *[false] {""}
            }
        }
        [none] { $hasUnderwear ->
            [false] [color=pink]You can see { POSS-ADJ($ent) } cock![/color] { $arousal ->
                *[1] It's flaccid.
                [2] [color=yellow]It seems half-erect.[/color]
                [3] [color=orange]It's fully erect.[/color]
                [4] [color=red]It throbs, leaking precum.[/color]
            }
            *[true] { $pastThreshold ->
                [true] [color=pink]There's a tent in { POSS-ADJ($ent) } undies.[/color]
                *[false] {""}
            }
        }
    }


genital-pussy-use-verb = jork it (crazy)
