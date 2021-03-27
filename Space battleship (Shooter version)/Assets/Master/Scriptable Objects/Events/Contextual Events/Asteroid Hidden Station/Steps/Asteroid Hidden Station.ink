VAR choiceId = -1 //Index for Unity

Once in the area, your sensors don't detect any intelligent life forms. However, the advanced scanners of your ship do eventually detect an encrypted signal for a brief moment. A few seconds were enough to triangulate the origin of the signal, it is in a field of asteroids nearby. 
 * Investigate the asteroid field.
  
    ~ temp randomNbr = RANDOM(0,100)
    {randomNbr > 50 :
         Your ship somehow enters the asteroid field. After a few hours of research, you end up stumbling upon a space station hidden in one of the asteroids.
        - else : 
         The asteroid field was too dense, your ship didn't manage to dodge all of them. Your hull suffer some damages.
         ~choiceId = 0
    }
    ** {randomNbr > 50} Try to establish communication with the unknown station.
        After several minutes with no response. You finally got a response from the station's commander. He introduces himself as an ally of the federation. He proposes to help us in our travel.
    ~choiceId = 1
    ** Leave the sector.
    ~choiceId = 2
 * Leave the sector, it's not worth to put the ship at risk.
 ~choiceId = 2