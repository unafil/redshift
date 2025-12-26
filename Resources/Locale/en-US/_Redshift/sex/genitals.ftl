# examine inputs: ent, hasUnderwear (bool), arousal (int), uniformType (string, either "suit" or "skirt" or "none"), pastThreshold (bool)
# might be worth finding a better way to do this
# todo: multiple randomly-selected genital nouns (cock, dick, penis)

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
                *[0] It's flaccid.
                [1] [color=yellow]It seems half-erect.[/color]
                [2] [color=orange]It's fully erect.[/color]
                [3] [color=red]It throbs, leaking precum.[/color]
            }
            *[true] { $pastThreshold ->
                [true] [color=pink]There's a tent in { POSS-ADJ($ent) } undies.[/color]
                *[false] {""}
            }
        }
    }

genital-pussy-use-verb = jork it (crazy)
genital-pussy-examine = 
    { $uniformType ->
        *[suit] {""}
        [skirt] {""}
        [none] { $hasUnderwear ->
            [false] [color=pink]You can see { POSS-ADJ($ent) } pussy![/color] { $arousal ->
                *[0] {""}
                [1] {""}
                [2] {""}
                [3] {""}
            }
            *[true] { $pastThreshold ->
                [true] [color=pink]{ CAPITALIZE(POSS-ADJ($ent)) } undies seem damp.[/color]
                *[false] {""}
            }
        }
    }

genital-jinglebells-use-verb = Sleigh
genital-jinglebells-examine =
    { $uniformType ->
        *[suit] { $pastThreshold ->
            [true] [color=pink]Merriment is showing through { POSS-ADJ($ent) } uniform.[/color]
            *[false] {""}
        }
        [skirt] { $hasUnderwear ->
            *[true] {""}
            [false] { $pastThreshold -> 
                [true] [color=pink]There's joy and whimsey poking at { POSS-ADJ($ent) } skirt...[/color]
                *[false] {""}
            }
        }
        [none] { $hasUnderwear ->
            [false] [color=pink]You can see { POSS-ADJ($ent) } jingle bells![/color] { $arousal ->
                *[0] It's flaccid.
                [1] [color=yellow]It seems joyful![/color]
                [2] [color=orange]It's decking the halls![/color]
                [3] [color=red]It throbs, leaking jolliness![/color]
            }
            *[true] { $pastThreshold ->
                [true] [color=pink]There's a gingerbread house in { POSS-ADJ($ent) } undies![/color]
                *[false] {""}
            }
        }
    }
