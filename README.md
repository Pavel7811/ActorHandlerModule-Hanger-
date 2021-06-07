# ActorHandlerModule-Hanger-  

# Команда для сборки модуля:  
git clone https://github.com/Pavel7811/ActorHandlerModule-Hanger-.git source
dotnet publish ./source/osmlActorHandlerModuleHunger/ActorHandlerModuleHunger -c release -o ./release
cp release/ActorHandlerModuleHunger.dll .
rm -rf source release
