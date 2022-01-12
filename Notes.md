# Authentication

## These fucked it

```
                {char[8]}
                    [0]: 31 '\u001f'
                    [1]: 122 'z'
                    [2]: 61 '='
                    [3]: 8 '\b'
                    [4]: 43 '+'
                    [5]: 27 '\u001b'
                    [6]: 91 '['
                    [7]: 65 'A'

                    {char[8]}
                    [0]: 162 '¢'
                    [1]: 212 'Ô'
                    [2]: 140 '\u008c'
                    [3]: 90 'Z'
                    [4]: 100 'd'
                    [5]: 203 'Ë'
                    [6]: 97 'a'
                    [7]: 74 'J'
```

## Workaround to get mIRC working
for (var i = 0; i < challenge.Length; i++) challenge[i] = (char) (challenge[i] % 0x7F);