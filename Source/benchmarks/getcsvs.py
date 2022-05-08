from pathlib import Path

variantDict = {}

#variants = ['princessqeforward', 'princessqe2forward', 'princessqe_recforward', 'mathsatqeforward', 'mathsatqe2forward', 'mathsatqe_recforward', 'smtinterpolqeforward', 'smtinterpolqe2forward', 'smtinterpolqe_recforward']
#variants = ['princessqe', 'princessqe2', 'princessqe_rec', 'mathsatqe', 'mathsatqe2', 'mathsatqe_rec', 'smtinterpolqe', 'smtinterpolqe2', 'smtinterpolqe_rec']
variants = ['mathsatqepassive', 'princessqepassive', 'smtinterpolqepassive']

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

        if 'timeout' in line2 and 'success' in line:
          line = line.replace('success', 'verifytimeout')
        elif 'success' in line and '0 errors' not in line3:
          line = line.replace('success', 'verifyfailure')
        elif line.split(',')[0] != 'success' and line.strip() != 'timeout' and line.strip() != 'error' and line.split(',')[0] != 'failure':
          line = 'error\n'

        line = subdir.name + ',' + file.name[:-4] + ',' + line
        csvFile.write(line)

for variant in variants:
  variantDict[variant].close()
