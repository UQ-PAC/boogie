from pathlib import Path

variantDict = {}

variants = ['princessqe', 'princessqe2', 'princessqe_rec', 'mathsatqe', 'mathsatqe2', 'mathsatqe_rec', 'smtinterpolqe', 'smtinterpolqe2', 'smtinterpolqe_rec',]

''' variants = ['backwardmathsatqe',
'backwardmathsatqe2',
'backwardmathsatqe_rec',
'backwardsmtinterpolqe',
'backwardsmtinterpolqe2',
'backwardsmtinterpolqe_rec']''' 

for variant in variants:
  variantDict[variant] = open(variant + '.csv', 'w+')

pathlist = Path('.').iterdir()
for subdir in pathlist:
  if (subdir.is_dir()):
    for variant in variants:
      csvFile = variantDict[variant]
      folder = subdir / variant
      for file in folder.glob('*.txt'):
        fp = file.open()
        line = fp.readline()
        line2 = fp.readline()
        line3 = fp.readline()
        if '0 errors' in line3:
          line = subdir.name + ',' + file.name[:-4] + ',' + line
        else:
          line = line.replace('success', 'failverify')
          line = subdir.name + ',' + file.name[:-4] + ',' + line
        csvFile.write(line)

for variant in variants:
  variantDict[variant].close()
