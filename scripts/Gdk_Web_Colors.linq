<Query Kind="Expression" />

@"White	FF FF FF	255 255 255
Snow	FF FA FA	255 250 250
Honeydew	F0 FF F0	240 255 240
MintCream	F5 FF FA	245 255 250
Azure	F0 FF FF	240 255 255
AliceBlue	F0 F8 FF	240 248 255
GhostWhite	F8 F8 FF	248 248 255
WhiteSmoke	F5 F5 F5	245 245 245
Seashell	FF F5 EE	255 245 238
Beige	F5 F5 DC	245 245 220
OldLace	FD F5 E6	253 245 230
FloralWhite	FF FA F0	255 250 240
Ivory	FF FF F0	255 255 240
AntiqueWhite	FA EB D7	250 235 215
Linen	FA F0 E6	250 240 230
LavenderBlush	FF F0 F5	255 240 245
MistyRose	FF E4 E1	255 228 225".Replace(Environment.NewLine, ";").Split(';').Select(s => new { Name = System.Text.RegularExpressions.Regex.Match(s, @"^\w+").Value, Color =  "#" + System.Text.RegularExpressions.Regex.Match(s, @"\s[\w\d]{2}\s[\w\d]{2}\s[\w\d]{2}").Value.Replace(" ","").Replace("	","") } )
.Select(w => string.Format(@"case ""{0}"":{1}	Gdk.Color.Parse( ""{2}"" );{3}	break;", w.Name, Environment.NewLine, w.Color, Environment.NewLine))


