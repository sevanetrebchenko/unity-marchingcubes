cd Build
git init
git add .
git commit -m "Deploying..."
git remote add origin git@github.com:sevanetrebchenko/unity-marchingcubes.git
git push --force origin master:build