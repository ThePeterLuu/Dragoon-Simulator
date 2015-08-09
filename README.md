This tool can be used to determine the difference between gear sets and to experimentally validate rotations.

Settings are configured in the DragoonSimulator.exe.config file and are fairly self-explanatory.

The Rotation.txt file defines what your skill order will be for the duration of the 3:11 parse. The part specified after "--OPENER" will run once and then the part after "--ROTATION" will repeat. If the list of skills is longer than a single 3:11 parse, it will just skip over the extraneous parts when running the sim. The standard rotation supplied has "END" at the end, which will throw an exception, which is handy if you want to ensure that you don't have a repeating sequence and that it's long enough to explicitly specify each skill.