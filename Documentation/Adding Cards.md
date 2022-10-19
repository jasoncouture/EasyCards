## How to add cards

Adding cards is relatively simple:

1. Create a new `.json` file in the `EasyCards\Data` folder. That folder can contain as many `.json` files you want, the mod will attempt to load all of them.
2. Follow the example and file format description in [File Format.md](File%20Format.md)
3. Place your card images in `EasyCards\Assets`. All images will be loaded from that folder! You can create sub-folders if you want, but you will need to include that in your `TexturePath` element.
4. Start up the game. If loading a file fails, you will be able to find that error in the BepinEx `LogOutput.log` file