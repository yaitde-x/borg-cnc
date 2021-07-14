
fp = open("test.txt")
line = 0

while True:
     line = line+1
     nstr = fp.readline()
     
     if (nstr is None):
         break

     if len(nstr.strip()) > 0:
       print nstr
     