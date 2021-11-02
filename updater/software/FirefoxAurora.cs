﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "95.0b1";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "1a16e645d9953852645f47fdb6e0be9bb4ffcb116a725700c0f42c14dd87cd6fdad4a5260a27d0e8ede62509ab444419bff9eda3e986035a3b7b11144a99f511" },
                { "af", "8618ccebb2338f64f031ed91231e5aafac87fb4074d51419344424175278ff73e9674f9806d9e30fa18005d6125392e8f3dfa284d4ae42de610d10d2ac0b9759" },
                { "an", "35fc5c9341ef081473699c8188fd918e8b750c8e6f00326cf64417f9536a6de5d48a97751bba59ccfc92b2c32124e7dd544751dc775b15ca3e8fcee79155d4af" },
                { "ar", "74a0bc3d363bc13341536465e4d3406731983b2ac1bca612e1525f5e1a9a90ee3833fbc71ae4566341d6b9b30da0624b4046fe011cd2b10a224ace1873bb5e4e" },
                { "ast", "0c5a15e8e6bf7a49f53f64b193a4da73deca7c83efb724309cc891a3b7048bc77593866f24dc785d2fbb603f5ffcd14ee6349078040f47a67edad7a89b125869" },
                { "az", "467b46fbe58e295a27959c63087dd20ecf81d4679749445f24c204f9fdabc0c1f1b199caf4afafb3849e03a3993dc0b7d7ab9623b4b07229a0dff27578f3676b" },
                { "be", "a5cc04a1565a222be102cf7912465f2b43c795db5e526a28dc2a5ec03fbffaee044777f003fde8c6083b6192f75f7d1b9b2b8ff1b70f25dfd36216dd8c370486" },
                { "bg", "59266dfe793ed626f1762339d92d73f0e9b5e2bec833d66c7ea58b9cb1b1b05a927f840ae77f1aa1cf3f70ecf25a5a39dbd83b0c9b8a073d58c33def61e3f6b0" },
                { "bn", "34510fb242b784afd9f81e42b81cdaaa7ca125630411f878a9e04661165e260937d93610fed59cfbfd3fe85cc5b9ecaf52a2d1ec43240cf686a43ccd7e017613" },
                { "br", "bbad43d0029936dccea820705c7becf3d9d5e87523ccdbe9775579e8aef1767cde16943167d107e28f212243ca0821e9dc97492ceeb941dc047eb07b5dc23270" },
                { "bs", "5a4a484fcc836b719a3b40154e40d712e57620bb2a4cb5678aff82abcc6c39f5f8700d25ad0330f4d70f152c538cf7ba0ca7a77f8293b91bc870ed636b7ce536" },
                { "ca", "4d507de9b6d94ad68f8548deb57574871c81976146b1ee789e244a2c1f5583628529080103cb6a7564439247ce8eea365020c52bfe1e7819ea17f2278b6c22a1" },
                { "cak", "c5c886af8852e4408e806d52c16b15a1b8253e46116d53ae7f7f7140f746c5ebe28b7543d6e19e1587f077513a02dc420342012e37e1be6f752a98fe0fa147f7" },
                { "cs", "2160d2f50aba85c73cf89d1ca1878e501e27c9d474c3e87c1acd4865ba635fc55305595bbcf9f3e516c41e18529b6db6e80eb38e65c2d6e6aa21434717917714" },
                { "cy", "cfcc2788a82d779fa946521f7cda8d45bb9419caf1d67136e551726f168ca022ea8f6c0bd7a773c6dbbd64faa8f2744e8fd67b3053c92ee843e80ec99b138c27" },
                { "da", "a2f37bd9ce60249873cd6657767e7c042d96810935dc30201af98657a8921817b99453b5eb4e8462f3dc360c269f711324f41c38d43438cf6e87464bd31b00af" },
                { "de", "91dc2fb6a334562c5c66368af260f2e7929fad2c38c424a77afd2e6c999b108e2251374e74919f85dd995a54991c3a8caec3a6f9a6a542224572862a612c5c62" },
                { "dsb", "74d268cb08f90da7c75362caeef5b74be53061d376646b06c8dde19d8b633bebcdf28b6dfca9f8ee5147f72444df8c2fcc83b42b88003d019b5817bf81bb38d3" },
                { "el", "f8c90622c68703608d56b4e972659d9380a0e56cef3653243c191b063892496d3aba13c8167e61361b2bb5c15a31dab2189bfe3497d0795de25bbe54811dbdc7" },
                { "en-CA", "b0d9ea30e2ef2dcee2a7d603a80ec8c7335e6b215d78cc5cc8abc2d39a92858fb9eb6631bc3af20538e80f8e609591a91613ba3042ef38aa6dde868adf38a6ca" },
                { "en-GB", "6e16948ea51198f18ef2781a2f96906539042be8fa1c276ead32ad85b74fa4b4ba6774a7c3061924d98029d78717348d9f48503d553a62eaae86ef8f5efeb080" },
                { "en-US", "eb9bf98edb608686af357e6ee8b078e359f29494f5b9f9cb66aed580ca4cfa2a7534aef6dd2069d5081aaf54ea322f7256756af069cf3ecfb40a8cf92ce1a42e" },
                { "eo", "d86b934a2acba473ce879fb00cce857a6a2360e11a9eb2c88c5befb4a157b30c21347963517bf524bce34fcb2f93c6dcb21998189253fdd07e66744c97dbe581" },
                { "es-AR", "3b6e74a488d3891406da91052675a3d55a0b709f3fc1f330f11be3a4ce4ac249ee878788c7132da5c7b0e7542862fb15b0b9fd292468e2081ce02b64360e75da" },
                { "es-CL", "72ecad4c75262bdcbe95d5476533ab15e29abaeee63b4cb4be0b9385fcdafec862fe155436e4d32935034e4ca5fdd2fc9ade7c2325b8658263f2dfa408bb876e" },
                { "es-ES", "fa869d0c3e2071e658b4d7bac80988dd33d9e3e3edacbdbab0ee59af8d91c4bd9105d9f587abe5225d2a6da01c4db0d72bbc0a3a5ffea86464ddbba6126daacd" },
                { "es-MX", "dbe8b5d94f22a846a13b1ea87f130d4180c9b7f2eea1e311c2f7d5fc7fe9a6239003ff98c408ac342ca30809bf2812231af7bcfc1a0e408feaf9d3fbb469a641" },
                { "et", "bd602f36b0a4cacd155a86c47c737299753d7aa6b5fd2259befcaa50438512c5fc021ba62f5322a63b5af268ea6b86c320924fc1457c4192c9fe210eb156ea2f" },
                { "eu", "f25c6cba33f4e4c1299c30d53a43ca3fb674851dc866970182d1a49b3aa6268c4c40839af3b903cd08dedd3e176c1fdd94611107722ee00fcb0e7c8725c105cb" },
                { "fa", "ab9fe7e815db40c13686e40ed439e5a821ab6902dd7a13adc2ae881028a56aa1b298ed82e66185d0dc5282f19b2ba77a23f4203b09dc697ca4f8f806a32c2771" },
                { "ff", "0a503483932e8b88bb6750c7b2db78121ac1343f812731b567b8830416e6b954d8e30986fc0936df3bd072355db03077813bbc9b3fd99c04685246aff5e70383" },
                { "fi", "0b42141852c10738aa9e0e2080058e6498583f963dc164b9b9a616c4020f834f4473ce8a89c468a62ecb44193ace158f813056c12d51900b9fa543a281180612" },
                { "fr", "ffa0bb8e3d56e9b6cf34d309f5230a282d31c5cefc71c89c81f487248ec94df93916314763d8de01014a35a8fba668dfa7dadd97a5e5b17bb7a5f6b2bb7d8c43" },
                { "fy-NL", "b30004cd59d01c08b647bea41902a17b9d661150914a45e89646dc6813056f944eecbd71dd6c7d6dfb24b4b411ebec4096af408f3257299cef55461600ee9ad2" },
                { "ga-IE", "78e5fd32e8c4fe8ec88cfcfc0017421f25a2400d88273d703c66c59fb566609d9a28b1b517e99c4e4c49700ea1c6429e4e4cea6c961308b37e86288efc73fbf8" },
                { "gd", "6ed08be5e8790b314056875540591f401a94cc898e44f4e382d4b5b5bc698c903a2b972d07657616dec8ea4ddaad833a2a6931fd95a6dbf96545b2e93ad14068" },
                { "gl", "be9c34b498513d6e25025283f65676efa54e5581a2ac3882efaf56f9b1866a7ea2966f068aef254eae3131cec6db22ad8b32e1055db18928aacf32dda42e77ac" },
                { "gn", "4f8d88817384955ca2669246fb3271fdcf6afc9b5cd86f3e7c71b92392b5ff009c938a5ff6151900842871a1f07addbca2990665c144f614f3b7f796cdb1a5be" },
                { "gu-IN", "078f7b7d13f9feb8b107c2c43079e3e38b50f0422f27c9aee02d0bc836e5722990c3fbaf26f43890c5e8885c0668072a454c4707863893407ad3a5403f3b2734" },
                { "he", "bab5c6bbe3084bb3b9ee5b72ad5e90b92ef93203b655d923fc2c21119399cfb9f5ca03c64de2e6a11b119906e150e70a251d8a63d409f97f3a1375457e6e6bff" },
                { "hi-IN", "57ca6b0894173abc260bfc10d71a21f5853050133e1db247ddebf278cd011fa21c94b6055a7d49a6b09f8c99e77ffb30e5cb82c1b84cf7c0bae97a13c2b4747d" },
                { "hr", "d1bed5f62854ee072c4dd9a7beba3f259ae907a53bd97900d1c294724955b33dba9ce2348bce2964548f671b5a1f0f3390bcf35a7bf5b894cc8bf6d2c8d7bca4" },
                { "hsb", "3b563f71b12ee859379bbd44e2ea8aaabc5b0e24cbecf169b9ff20b70ce1113ce3359924a5d00c53a2b663a7db691022bc3b95c527705036ab385b1122bf5ff3" },
                { "hu", "c333b471862ae5941cab970d6c730fed0bb6aa7c5ccb8413bc422b4cfb49235e4bdd08672cc0aeb76ab579cc07e01fe442b19ae28d74b4a962d27c7950c60fdd" },
                { "hy-AM", "8d4ac65955a5d40efc41ff0dffedbe0fc0f2591f19e916ca91e4da1a8358d1a78e48b930f647cfaa65b8fbb6f025f747314691177f734b4f3628b8cfb79159b9" },
                { "ia", "170a54a383119960a4456cb3c983cb3d13d91ba639e0e4ffdc5d09a767429f1a2702d0f1fa231470daee228b56badbc95ec3e2a22a38e8c443b0cad1f6f3d579" },
                { "id", "a86d0b0d8fc16731bdcfb029dc7f371193f86dda2a4875df44561c6749f9661aef3e911e6a9b9c86708725a0078dd6602a5d1fc1f6320a54aee0cc68538ec2dc" },
                { "is", "0b6bbd3c5b4bac994cbc5297b155ad166e28e92e28f1a988680eb9d0ffaf5088c48bc3618253c0c2200a48dac948f0d57b71dc26702bf70858c619166f70e1cf" },
                { "it", "428f0ef4b68e462bd9e29545b77d8c1573a52f01d7bc4941f32b188577250492d22ac809e44270a9799fca9547a1f9db68bbfe0b4b2075109b6fad06bfcece44" },
                { "ja", "e13fce77428c170bc562467e8dc4e5dac5c88288711d2f91f2a0fa73a9529b5ebec984ae632a6db617b85c3141c5817fac2e8c3b6b8e2c918cb47ff4efa281c2" },
                { "ka", "8f8059ff5e0f33ccf4d19e412dda4533b9615d162988672ce485c404fb57fe326be8e6ec00c02e148e44e8d93bef6223e419cc87c77318f090a950c9d55e4d90" },
                { "kab", "dbac17af82e0550298b3f3dd487ba6b0da2befffd52397fff5d5f42f5a32953dacb9e57dcb810ac6f5e6be335fcd13b0d4208f39fae8df6bd4dfeee30480a88c" },
                { "kk", "41ab95f9025383e321e5abc4e9f806b245563259b9dea1fe21a48c667eddf8aa316937669703cbefbc1aad59805e1e384de9621eae087540a4db4fa37c466cf3" },
                { "km", "1fcad0c4178b71f7a3f3941d05cddccbd5fb64290c8b49274a3057bb1ec55bdcce04bfef539995d80549d130b4cdcb679c578bf352b16fa45300448acc8dab91" },
                { "kn", "c84d5bf7231e8ed77a1794a7ec11144f0c6179639351d6f524ce9459314bd293a8cf201c9c30ba0f1f01ae8207a7fb4cf927ce0e8a38f677540ec750f4b86832" },
                { "ko", "d3a6747a8e83668005d94efcc2a687a39b42645a3e0ec986596b55ed673710f0791c45919e1cde02c766c81ceed832625cb05411a6878d1866316b74544bb83b" },
                { "lij", "2c0ff91edef0e2c2ad4e4e236d040428bbc0d49c45b0c38eb01c18de25c5ef05041a542a6de926c42610f54958cd5092123d2cd3d49aea793d32e8369660caaf" },
                { "lt", "87b15b51551bf0296d8f94afa0d457662fa000caef066aabb0b6b1f8a26f759cc57a559820a4a0d2578623ab0205754758e13bad64450550f5ffcf6758cc1eb9" },
                { "lv", "173b525f2cdeb80035f4c8967435c9f94a6dc027828b2cf6451b045c645e0ab6941ec30b84fceddf3e5602fbb5887ce81f1e67537e66b90a14654be9043380da" },
                { "mk", "a89ea8e8804003d6df6cfb6e96280877bd7f5bdbd334fefa0ebf61dafaef1335e13cf28b625ec4030b46974593cd89e3ca6db274d67f9b5ee1e5d453d0566608" },
                { "mr", "bb862b42df0a6d2413933f5d8ae25f15bf04c375567a0c7e220625c4d86f29293e6094c56129b3bd2d2185c1caf4923d94a27ad72b72760293c1fdd1391ae5a2" },
                { "ms", "9f71f40a5ee619ad83f41cd2b1bc6f4b3eca199aeb23101a5f4eba4d051bbfa3bb4ab2b1750223772753904fc31069c551624b4804fda8b383343f098b718746" },
                { "my", "866813734ca11e45ebaf95ed01b4673e6ea595e5725fffd822aca2d8732d22cf5969fe210a451a08d4f2a75a2cb9458ba04198b792b107b37f98443a48ee7878" },
                { "nb-NO", "5b6708d7454b8b3a5f5351026f49798ba127740e06777b4935030b87cb443cd147a1830f4bcf3f8b73bed027f290f996299fe1336ca77343bba0567272c15a47" },
                { "ne-NP", "37b34d2271a0f2eb448b58de657518edb0d5784927c194c3cd500f9338f6f07d89e391569743b8a3254e49d2b8f90685366a9f3ef6be04e54bc532288095d335" },
                { "nl", "4d1752ecbc378001362efa99f2ad1cba2f68c66b264e9659a1ff6d58a5c399f019738d501d76220365cdc5e07ef12e9f57b7aa2ec090dcd040a6b3e63e723d1f" },
                { "nn-NO", "42746cf7983732e236a5751a0d5702ca623e06683518e5be8aafa5383494c4687a2d638bc48b9b8e7e7cde7fba21b2afeb915f042497d362def04697bd17eb91" },
                { "oc", "3c696e52d45d061105eee320abf0f0034bb45f390d26561a5321c6a42a3686d6dbcded52436be62e25046c18f052804f9947931c713371be5df41f160e96ddea" },
                { "pa-IN", "7ef43a6aef6bc1e11d9e07baccccef9af1e7a151f7a3cc204776f5f2776b8439a2b39f09c8b74a69526efa8797801bdada8a224e5df9c2ce20ab74beabf39b00" },
                { "pl", "746b30650a9b619ac4352d4f87d3337797fd6247d20cf9ec13b14546531cceb0b62787bd6bed15f779d2d6ca97e7a2191cb1fdbaf2c8c00ec2c02c7db2ed7239" },
                { "pt-BR", "98e8de3d5dca40b37b528e29cb70146cdff81fe88e83319553ca8cb291180968ee26d33852463a36433f8ba67af7e9333ab225db925388863ec7d0f4a8c07552" },
                { "pt-PT", "2a743c5d26f26758c08a00c965b8e160e3516bb6f4ccaca447bd630771678800f137d8ad99ec31fd4ce5d9a81d17362f19217ff1a72d445ba9f630874a4adcd2" },
                { "rm", "92ddf602e5d86bc78df043de335a7372e3d6793dc83c7c22b17d0307fd65c637b57c7aea3178539c2c575d3d8a070ad945f18d98c43b3729d46e19fa089061bc" },
                { "ro", "e5fc0e1ca62dc4e2ebc2c95ed55675ed9daee787098a1ad5e09a7e0c07b4f54718b481e64c714e6b71e50d72b937a47ab1e001750c878ebf9420c1557786b699" },
                { "ru", "63b93412968a8f4ad941d55758bd13a1f583bfa125d7f72b931abccabf30641769068f8cd0ca7e27f9115d3f6f38a8930e1b48c196a3729f6689c6701742f67c" },
                { "sco", "9e2028a4d24f67d4c7c3ad1033973cd0bb7937861febda5c9ff4c5040129d7419bb4561a681f4a75ed43284be383a68c84b5d6a0ea8b5b358d1c33085df6715b" },
                { "si", "e44e391b464fa3b1c0127e328590bff2e16cd2c6a002c83622300768b01c58fbf1c8917eaec7a2d505f21c91b6f1b3c606fd7e9b18e902cc4733c6699c9de218" },
                { "sk", "ce0412b494e1589abf3c39983bb803406c2ba36d1e912f69940df7fc1e3c9aa18da52c291de6069b1e6d4665b5d6479ffb8ef3aa32c6a0c15c50fc3f4e59e478" },
                { "sl", "6bd75cfd140f7eae71a0e31ca8a991ca5e39225679b98fddfb02e40151fee09e1d959144e8419190e17741cd0a7a90b409c30c0b332f3f11309b658c37319d0d" },
                { "son", "439f6c32b2c65dfc6724c934618990b1a7a2450020cb2f81203ae8db5373c0a89aa602aea508de4db1efdf751f55c2b50cd4a1e4a36ca733879a96795f222521" },
                { "sq", "20cbec1ede0c5d33234f42a0f414abfd51316241978c1b8caa8c209905b172bbf8a5214fd41c66ab6736a5f515b902e816b65e427ea83f2460320f24bd4c71e1" },
                { "sr", "18ce2cd2a642ff5ee664ee6b130181adcbca188522fa8903d012c8ec08fa916cfb54241160cddd8723edde2280471e339a32549291becf3099cec9fb0338f84d" },
                { "sv-SE", "2fdc33678ab50ce20a59f1c600a9831c8f2d99aa7a738d1e6f506023255e34563ffd8e8ab58d8eeb307ab4b8c7b12447f17927fac0569659556284407580d821" },
                { "szl", "593564748f851f00d6a87f6d058f0dd55fe64e9a1a2ef71d51566fd4ae762cbb6cbc951f1cf12063b2bc0a4e79a3e1473b28f32595433627ecb4612b9732c52c" },
                { "ta", "d40c872b2d9a786074215814b118f6c492934848806c69e5174c51a869237690165b421f0e5b4d8c674b4cdca2a83a24021a9caf31421e0481079c6ed396d32a" },
                { "te", "4ccbb10f6f9baed50b6e3e2ba3d3178a149b8fd88b9307e8963a1f315a6dda18cc1e9170e4b35b6c36a149c5a42881622473dc5d06239810c53665d9ee2efe04" },
                { "th", "458a139a2ba6e06c2e1c6c226416e5f2cfe406bf98e9ab61d3e5a35bb0d51e70abc16a69aa97d35ef746a7a59b884f516f9404b205ff484e3257505a9073274c" },
                { "tl", "42db7f6285c0272d6fa10bcc7d9f256ab9211a6289ddfc77df7ce7aa4eb712d976e5ef3bd53423d9ed79e7c2164f5fb862a893ad55e85c1a05b1d2f02bd42b32" },
                { "tr", "7a907dd7f08cf97766e97c97744a2a2681502d5a9e6571a39cb4902de94bc263406b21802c7a0bed0fe8d85f3ab66900f55e0ac8970a58c49c41e98eba204903" },
                { "trs", "1fe3bc7cc8d03cdb7c7639e62c717b340190748e8997475648926d382c529bf1964fcf33cc5e0c5b301487d4dea8c9814103c79f8cedb85653aa119a92aaa004" },
                { "uk", "5934fc1aca9c2b8f458807095751aea4342b14319878913f79720b620d72b6e48b09bfae006b11a4bf1111aa31e12968d67d5a348f7139ea7bb9ac3a9c9ae675" },
                { "ur", "fc4f8d11791589bf821b5c06a31d651067ae7380fd22632e50799a8b78261260bed7e157867c0203b3cccb9921661c7fa93020b3045958cfa49050740b1305d6" },
                { "uz", "8f7eb1d02c2654c9ee403b4591776b671130ad5ac756360aa8343d2f03f0f113c1f3ea471b679a92dcd494b36bfbe8363f3f8ee158d50ade59b378b3e7faf747" },
                { "vi", "f26a84b38496e387a6aa70ac8a49d60cdc7b8ae90f4383479c39bfcad051f6e0fd5ead500d0feb5c7c5598725bdca0f8c3e3bb2391e998ce6a277f475c6891ae" },
                { "xh", "6b9d2abaf5eba4be13fa95bab851215cd491e4ccb9f074ff6ca1da56a254a38aabbb0affb18568b813cbae0afe82add01853e217c49bcfbeec32d9d75abca3d3" },
                { "zh-CN", "47f10c402cbcc161a384a7a3e830c981a9db42bbba5a7ae51c124d9223e49b0760e77b24ebc6634f5b3a51fcea996611ac9c8a4fda55a2935b00481a5f1cb879" },
                { "zh-TW", "b4d62cb61e213c5fa6a4c50a224486f2ee4d07f6f1e991d4a4883cd740f63a435316f33801cc96c97cd5860a210002d983d9919e58c3c430a5ec05e1f03f64bc" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/95.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "9fbe3b049111618336f010dcd6ed0c8c1a6bd46aeb00211c8932aa9dab21d9def0d6cf97b4ab280a88e05c7e1711734a41a494388e2f99e0cc919a133790e3b9" },
                { "af", "b910b90c91e156f2028b03c6382feb8c3eb8ac4ff6d5bb1ea9818804d4e069d304e76808e96b54d873dc0e04675a98e276dd06f9673804d0f140ba78305421ef" },
                { "an", "1946896ac0ef09d2f144a35d6f47af93bd39fc8afd24c2382bb9c908c31ebe9accda6f133ef1e838fef30b4a643ed222245be303a5913f9e6d131db2b4bdd210" },
                { "ar", "334b109483c6dacb4095d489ee0091d086446d6e6da36522df04a25ec1639096691a1eaede1f9618502bcdf0afacefb559519b33552292d7c5c7ee260f7ef082" },
                { "ast", "c5879e452d75b5a48ea2375c083e3aade23fb04ae603f0ede49610d52e9a068dda1af6a939c1e27cc8495275171dfb35ef06c8bd5ca669564c4d341ebcac8dba" },
                { "az", "12a7ed008b443f704450acae93a01bd7a9decf1c5494dd4557e5fbcdaee38ead441e8e388f5625339b00c45bc8d8a74fbde19926dfe9fba0906c7736c3010aba" },
                { "be", "a0832dc5e86c5ce42e863ba9f3e89627795f50c68861e2280373d512e2a9d9f9eca55b7afcde38843b53e653f5f52e6cde0dd18ec99aa7a10baaa87e110d4e35" },
                { "bg", "063e6b3de6f3fb1290f3e7fc35881008dadcf6035bdf5ec5a57782f1f19f58c072c3f441a96fc3cb71407bb830b15d6d5b2da67b68e47c1f0422ec7c299cb8b1" },
                { "bn", "cbdc798619f8a18b498ca7e02bb8844733afb0259c6736aa21d0a1ee512c2e1c1ece83007f94fca8aaee0ef5322c676c68475b1843a26176c21c449eb86dbc3c" },
                { "br", "18e7ad4a46f67fcfaf8544ef2b5e355446a7083defbc9755ca7b5a9c7e0d8ae8294df77b91412d99fdc30be52dac8e8e1e78e0ea71a65bcb7746af95aee0c15c" },
                { "bs", "346554c4a32f9db739fde0df98e15e71f5a7288f188ccf9cd3254ed131103212f6ac1804374f0716875f02f582b802e07d6e9dacdca5636a06b55dca0088da5a" },
                { "ca", "6cb52844309df9a096473086455b52015bcc138db32bea1f268d4fcc478c75531329f964d3d87882f0d10fb745a1ae8b3afd3c9fddc8dc4667735d60d33dccaa" },
                { "cak", "0556372b8b7b60bf7bdc14346693652a954021031081f2fe7cdf91dccc4a1740136a5f04d71db9bac2f23ffd82927f381d36f84e5845e640399b0d7d2fa080b2" },
                { "cs", "35ba306118d6499763e3f77bd2512f91e2b2fc3f28dc074504c73aa72dfebc4c3f27e6159f64395a2b5a2d786116c7a4578c446c2a40b811731e7fd9bead7c70" },
                { "cy", "0486afa364cb4982827aa8c4fcb5749d39e369916372af147dcb0d445273139e1538ac38a12bcff168b820e771a7a942e53f62d6f3729e2d6971689df222c2bb" },
                { "da", "a5c669932bbc88769b2197c4df29665e34ee455c5a1b4c2d8ba3631411bd31eafe21df6d372f78e276bd2ae65f2af26cdcd70b4d006578f3780b4d8dcc6b419e" },
                { "de", "bf9a36244a4a86cfa77a426e730b3dce113a3ded79d78a1ce9f7e1db1069a3c91ae3be269a8b0446de2f32d10c5d2aead3523a21d28d894d5c8ae3b1c32c6fc2" },
                { "dsb", "6890afc2c47a707d101dbb82dbc3674a8e64e89b633cb0000ec25613a4b8bf9aeceb65a07f35bf14f1380c62455aabb0f8fd8a00a4a23e0ecb80c7f31ce98b28" },
                { "el", "46a61bf6d68148d18a471cfae3a19da36f2eb603b124bd106cac7b760663cb09a4407cf6106aea31bf4038c31061365c99906317eeb99469cf22401e347bc014" },
                { "en-CA", "d5e89a69d1be6b60e4ab1650c308c1ec34932f1cc72960d6aa005237407ed425069c20fcad406ccf6fbae1bd1195ca5e6c9c35808747851fbd81970f14c4192d" },
                { "en-GB", "59edc49e8caccc2d39de66aa28857574eff8f3be825fa7849e2dce5fff3edf74022d204a95ac16c607c29892e0dbef72795d87d8f55a96ac181900962e7a0209" },
                { "en-US", "8b99e448cf857e2e70ce6c6fada9cf85a4eb7f693f129501d45f258d84be5ff1f96978d3c7be3f5823a05b852eecd61aa5eda5acd0ee5e4d65a06aa356259c18" },
                { "eo", "55fbeea6cb4ce26359a17e50bc9bac7042403037dbcf39952837277ede3f99305587992ca2f95b8c31e0b060a55dfe9901c4cdf2dd7565f92248c1bdbb6444f6" },
                { "es-AR", "b550c8fd67b3c6d2916b88d0ef67ba42a1bbc7d6a172ffebf34edb8eb007a9c6f989c8633e346f4a5388aa8ce4e897496e7dde42edfd27594d4d2bfb60210865" },
                { "es-CL", "d04ce79484e8ac731070f926400106c69510706cc1dcf919e079e4df772fd2918548b20552c6536b2a942223a60e0d01dcabbc42c61d931494228739c4cbd6d0" },
                { "es-ES", "baa7e8eef0818e4a403d6e68d834003e57a2483cacbcf6b2cf03279369334d2bb8845a72e35ca92968071dfa8e6a7a94891ab6379c4de1f4deae306326e32116" },
                { "es-MX", "cbb6996e7df8f40152b6271c4efdac2372ad91cb6670207d62b1367e5c4eaf043cef5082060238f193158fa2a37c72d749e1d12b856a355ba1c237adb5ead9c5" },
                { "et", "8443c93a35cad124a15773b8050a8197ce0f742e66333f3d67d526692c71cb7f16fe52affdadd3fa9e92012464258f00ef1a6fa97bb9f6c61f74ed730f88d182" },
                { "eu", "f90ca9ad411408cf46ebb1156a0024a088032fc2445f4a8a3f65aa1af8e4191ccdd5c7f9fd32362a3682f68f3250b4026267151202352f66881243268a6c8b78" },
                { "fa", "3e957ec5aff9f9d062cd7583ffcee988b707a91425ec27869cfb5ece06d0ea3b2d1d66b1827ce788af3c60117368539d01857b23fce0f458232e0eaedcf39ad1" },
                { "ff", "73eaa4adac21ee2cfb8ab88f31630b973a2fa4df5262aca3273e36bfa58f80bc92cda6e40efb9ee3160cfc40ca87ac2a5ca4c5e34311f126b2bdda82a9f565e8" },
                { "fi", "d8b7357bbc725e402c465ea2d87af38f2a3c90a0bbf75c1827bf8ef5b05ae3b9742b46d08e91811e658649832573a56719ce1f128ea6ed4a4fc4dd27fd96083f" },
                { "fr", "44e12ca8d4c721febfaf5ca3617756171c594845c62e433824676026900152a4638aa234c210c1867c141385e60091d44b38f628cc4b44f3229006620370eaba" },
                { "fy-NL", "39148939c171b4aa96636df044f59cd0f3f318817f0abca67b54dde339dce4ae8cef872abd8a1eadcc35acafd3b4af4ba0a602e2be63a9b5e6d5d93a7a8bc129" },
                { "ga-IE", "bdb452132639b47c56f9f47d7cf619f4c292b01ba920441e5472bc4e20723a188e703574fa26396dd4010f3ba32c83de4ee6b9d957918b55c8b9842ed21bcbea" },
                { "gd", "3b67b0a18cfb533e4a9a5315140ef14558061c0249b520ca026ca8552589ac99339fa2107c6910d28979daef88377d4dd712e9e2b1c49b37b3db6fa91950cf60" },
                { "gl", "f93641896bd33cab521a38df7b2bf7064bc3716e26fa6c15d954b6f92c1d388095721010c86c8e1991b5955393a84cfb9d1a678e3b2e7a0c832eda9cb1eafb8f" },
                { "gn", "7fab6f15f1689efddd7f43dd04262e4df740d204f4ce78da66bf5e45e8cd7ef1076a4ff6c6977e3b13b3516f5ba01a063ad2aead6205bc9c09c870375f0d8734" },
                { "gu-IN", "5fff97f049e92babd3e5e52584d17a6fd04f275c03b27c32c1a0e5ca96d66bb18a74257d64cad50495e154eccb04031c93f6c004054a8d71f49c81b1d23c0426" },
                { "he", "f8901b5b1add40669d2b1214fc748ca61d2c3caf6e25a37e8ea242e3f525af9f38b413d0b22a63b60fd54773af16d89823829ab0aa325beff1da086f381d2c4d" },
                { "hi-IN", "1fead659fee2487205c0c27b4eb8dc8ee105c58d0a4321f84e0036f9f43ef51c5b5d5a9931b1988c20f335320910b79698b84397478347a88d9c4ce24545952e" },
                { "hr", "444228a4a1d184f053f338e98f262b79ceb5e15d14a86ce34d383f733759e6f503032b4bdc71c8d3ac0528ee62116d9293eab9f6b66abb3d47f6237f7bb2d08b" },
                { "hsb", "ad9726b52d4705751853c03eb55b23de4594efc1d316d63ffa90d5d0fa93682d3d5721b126c8812f0dfa120559ac44016109c9da1c6b28ea2d0d09df1f6e9b09" },
                { "hu", "e28ce6ebde2a20ce3c7c2801a4d5a4bc41ab49e9b4078308479564f3b281fe4481b40bc1f21c423fd2ba880a1f135cda627b7404e24e076d32a2c4aa7960340b" },
                { "hy-AM", "decdb7013e5c0f936871e400a94d2237650333cdd49e643acf712398aceb8889910673c39886603107540f3e02860a203d23997e50db0bb9229265cd243d9d0f" },
                { "ia", "6d8e8dad7ce58235874b4476291020eaee45d639f86eefd26d8a810a829468b7e664384b1d0d34ecb3a7fbb048f5c0eed8f7105ae7c4a51717764f4773b1a010" },
                { "id", "443bb209de5e0211b309653de592a2cd7630441ca6a92e7c1d3169d035b92a251fe3c0bae94bdf0d26bb014b096058ac8594eb437dcca2b7ae54c90bcdcd8eb2" },
                { "is", "6900ad8d732720cd06e611021995ae5ed2ed468db0a143d6f1ca5db2f7b1b15e6cf06755abfdce5f9c924cb15e6519a61963c87e1ab24f6249d898bd96ba0042" },
                { "it", "7d0705013d64870f8b897d5281b1c2df2a66f672e5b549d45efbcea54fa48ecce76e5c63d1f170ef2c4ac71942ac1daeac7209adc2fba3e7cb9d47fef8196558" },
                { "ja", "c8165250a88e8be72fdedde86f4ffe8a7730006728b5c1c1d378a6a01f40fc2f259f3af425f983379c1f20ca8d1474716678e051650fb91c983e3361b1aa7294" },
                { "ka", "55b3a1f7c2e95eb9002b1d901fe7c1d0d5654c437ce1d0418d295e6881053307bceed372d65f613fc17ea04a91a93cea85788ad7816575fe33be8660738a5a0b" },
                { "kab", "c2fd940572e9f0d952a39bb74e87d9fab6f792d82ca40e0f1ac669d997e97f1c5bfd0097617d9836468f85686b2a05aa835e0576730ee8e2b73b0b848c3d3634" },
                { "kk", "f78a711ac41480ecc9ee8da39c7a317bb2fd58b5847656af0541ba0befd40bf2d181a94bf7f15c8121ee32ad4acd6f6983c0276681267dc09eed5f40885e8d65" },
                { "km", "8c4a5408aa1d7bb817ed56764508c6a54c90fa38614fd7a2eefbf0605b10ec2afc9ad11cc2317e9c132328de5f3361dc68c9e020f03d0d4d82b861b80ab0f89f" },
                { "kn", "855724df4e8c0d6dc239ac6b11a4a613b4066eeccb8e7315b563442204719011d5b20185f8e409a7387dee7ec9f53dff85baf797d6afc8a8bf5e8dda6a5cabdd" },
                { "ko", "b0cc31be03bc1f8162cd5e8f57231fcf92f9a5b663cb88cb2b50a69a9416a3992c8ab4ad29d294f918afb13aa66ebe5843f226557becbc3fe6c1c882c07c9cdc" },
                { "lij", "d100399c8b8be151e5b8cba2ae70320004d16de47af6d6848d0fa95e5d9dd40668e4905e0a463c3cce986f34bb240fb8b80fa2955dec3b5fdcb489d0f848c718" },
                { "lt", "13a2d5727c7172842692dd5c338fcf014e7b0d6bfb2385fecf00974413112b4f1bc3eac8d61e02d41b95efead02912204234ae5e9786e5306d2b007cb776394a" },
                { "lv", "cef689b5124cb09081a7ad2918f54afe466ef22411fa6702f1279016c4dc7e57277b6650f7f1635ef55a9f175a6f488a5ea7775dfd08461375f11a0e71164269" },
                { "mk", "8c41e6e80a70367e2db878747d9a8ead476212e35ccbc45ce982c5f1447cd6a2bb736502d2189a7ffae343d695752e7fc2ee36f0e3e21183ef1c3db77370e4db" },
                { "mr", "5e64509c457c663c5130cfc52b627aa108879a6daa7e140eea554687e95d50756d1cc073dd9c4c0d82cd0ed83ab3ea9de63c9439080c8008fda9ed0cc166674d" },
                { "ms", "6120d38a67c5eb56b10ab8ca9f87ba6fbd7ff92a30770ba62893096954ec3cc8d1533362283dabdf9fea0b163d84063ddbbfbfb6bb42e7c8d4a31229e537363c" },
                { "my", "b56c38957b73f9ab859a866d5f7f37e953dcdafc0391a2906052c40cee1f4ddb7a95f59c20ae8eb81e4da9b12788a05e52b74ecbaa8dced3c90a16a100ec419b" },
                { "nb-NO", "cfbc5c026bb6d96136441b57ae6c3e33045a497f22e5dcad4f766accb302d5c55317d4fbdfcce29bde8e0747309c293a7463c6d4c617637e3b4007f032c9cac0" },
                { "ne-NP", "94af1b537368d5d495f7764256e5d774e9e5f48b8951fdcb0cc6a795a1401259b0140e8d88990b03ffc3b4d55434382e7727de08a3ab993619638a90c14cfdc3" },
                { "nl", "db2a6b8bc0c75861be9172afb83bafa90c9722ed3fe952d2c5feff7b4f6d98e2c7388d9d809b4c9d347bce8fbf1b419b8fb2120f266b09ae82fbb3405c2dc804" },
                { "nn-NO", "8d045b3f78a6c4b3030ea05114bd1e8b6b4ee99ccc2b906eb1c2bb0f49e881b040eda2957aafa5ebe019ab90a726f28fa444aac32bea9fee5aeeaef7c209612b" },
                { "oc", "793109e00c7de48304cb1ab580dd0a4019191fdd8788cddbecd11b56b4104f409cd6dfa812d4ca4dba398907fd7d397daa3e17d322c9859fdb761c23c57ef590" },
                { "pa-IN", "87f7f596804c5acbc47c1c6938ac0a96a2a38ee8233592f1b26ca370c3d1a19bea9afb7d581e35b29ff70393da10add934d397e29ee8ff6d37a7c6863815525f" },
                { "pl", "adb0633710784ef1ce9b08209c56a419f06df2caf957228495f51c4ddd0693323c7abfe894d0037062fee0eb589e89a3f0527932551fc618b0df33e7f06f42fc" },
                { "pt-BR", "6ed5e52978da75d7bbd94f9348b8e27ed80cb87bf8eb870afe7245788a374bd03a06e0cc0a1fcb4c4599ac6eda88eacddd500108c77b1b4ee7119d7f3b75d3e4" },
                { "pt-PT", "9865ef3554a08d7d4ad477dd1f651e4f02eda3e519ac0b53e253bdfa051235adbbd203ea1b22796f959dcd23092cc44a852f913640493f11a027d5f073297425" },
                { "rm", "3c70d7816c292f208cedd0cff7ac820394501ba21fd27cd62f57bef5b0225b3ecf51ccde99cbefdbb5ac0f144e17ce9b12f9f724f60cf89be8f58ba68c0ebaee" },
                { "ro", "bdd551980fd0e7b3bcb75d6488438f0133e7e324fae7ea9c59d1505f780abd3804a83bf53366e08af2aa4b2521539857e916b3d9be0608ca24e36ac7231fa995" },
                { "ru", "ed27ce5ebd55b2dc272c5ca00d9dd67577ac6a5573891d35da4e1b75c927e0b9567b4ee1bb107a8d96ac43022ca4baa830f73c97b30038865e08ee533ff6b8be" },
                { "sco", "3636bacc5c0796341bed112714162ae0c8bfe3ff47b04da2109d8fd9e0b6b5c078ea772732f1e400b3f2de07b12b2b1228a2d5d0ed7bedeeef91077f5f848449" },
                { "si", "9f633882d5ac0c09afe8d49615ca29f8cf9a2d0e1cf26023c494bd82535995be3d8f4b6aa7575705623c3870f28ff94318c94c8afc31de69fc2fc8ed3e56995d" },
                { "sk", "fd0056b996fe611fbb96bc69091e990c2b46436a1e668ca961ab427a5f714811675822c768961e9278cc5755101fb559a4dab9ed08825b41557de2a273779ac9" },
                { "sl", "b4ff40bfb89a6e3aedad0850964d74fbc30aa5eda5acaaf219443b3e749fd46eb975e8a4764fe3c56251f12f28a1f36afb0fe6e5f0b45d039b5950928aed2fca" },
                { "son", "353ebf3ea7b4e4c63f7ace35f5d3c4769ccd8a8190408a73cf59a140a256206867183be674670a526e151ce9fab9985831889b547cef65175ae1f91faf072dc8" },
                { "sq", "4ef2a34afb7535943f8bd0c37060a7f48bba5c48820d6d4c92998bccc324a1d85876b9b1a17d570062fc422df86d012ed7f51920661b966401d0d87d72ee4b13" },
                { "sr", "619edef8f7a130a62197c2c83717af84131eb53b0ed6384af49776b38c3391e9c03947cb55913bcfdf5ada5adc3e801ae32bfa7d1316265bd3214734a91dd6f9" },
                { "sv-SE", "49607059fb755ee8280ec5779ae63ad2d0248e1498ba069da0ca171a56437ceb2ae5e44c84c8be18f7f44cd8dca7b239e8b6eb5a3cd0c29c01397cb2d5c4e3cc" },
                { "szl", "d34fe160cd724b48013b5199b5fc1409d4f1f42c4646f61bae563e51d6a84b8c5f784491a872c4aa4e3fb5e0db6aa1cace8e114f419e3f69c758727150943e8c" },
                { "ta", "4523176bcbe9b55a29f9d633c1fd4443da44080fe9bbeb89b054da19763c7c897cde9841589c0895a4abd295f0d8ef3c5477b0929db7c297b410cf2a9b58612f" },
                { "te", "07f9719a16ae99eb53e208a684b9e74ff796ce58c1d1c26f2ed35e7226b458b9528c06130723f5cf962182959c9ddcbf08178321a9cc0781e7a7a6b739d7b3fd" },
                { "th", "d96755bb11f6b954cdc9416739cd322b0951d78673d0b002488bf98335fb8878b050a628850a3b03c3d7ea917b5a697923e4473a3b434b18540edf03a66e7efb" },
                { "tl", "cc3df8e43a9b991eef441fb695722242bfd02839c8cb15eb9b88f09758b11fcc61a811c5b657f2fbb9cada8e7b7b452c1a31a5a54442f63f0c2e5c37b8597a05" },
                { "tr", "eaaf4fdfb725ce182ad168cd2fa27eaaef412a40044c1c8a85a06009ec6c9ad376e19b8d16a5b363b9793354c889198ed50bf363abfe0e36e100931a0059c95d" },
                { "trs", "64f1223dbf8af2dc38a1e7d25f37a8c16ca619bea68cb6e3c0a5df603a368383272ab433f5179969c0365b6ce3f3752167ebbd0a02007fd1d638cf0f107d7e50" },
                { "uk", "d54cc7e6c9f22671bc1e48d574fcc93d5ce89c93454aeb6f89ca2a9f1bd3e995745451dd62efbbe782b9f5aeeeb334a74166aa2a2591578c7192fec6a65e534f" },
                { "ur", "7cbecb4de826a0e813c3ec109c2e08900a61ea5bfc613fc8f86f3fcbadbb0d3d7d65fe9750a1a603bbbb6b10821b470afd653ed2e7ef48314e0727002ec82d6e" },
                { "uz", "862e6428f9c00f2c758e20148d42ca68cfba46a91fcb756f6cf25311bb6590f8daedec5caf8a80e68c22700b81182e31ad9f04aa81f5ec043a5fa3bdff36bafd" },
                { "vi", "273d80060d69a845c521e3559abc079ee479b51d7a89c81ca311cba244260624ed484bc4969d9414baba6b6c89a4b24d5bcefb538850bf2e035ad94887f5fd74" },
                { "xh", "0f0dcd6e832cdae1a98106205250fa845e2287e92e300c14abe7bc1053d34dee946ba4822919d8c4bdc4d1ad43d35b6e6b65b4cd48bae40a7fa3a5fcb18dd54e" },
                { "zh-CN", "2ea1f8d4080bc0cd906212570867191e4e4a9b266abbe8edf605fb8793757503c55431b3bc5b0fe6c49d1071e76f0ee36f9583b0a654e533014395c07571c49c" },
                { "zh-TW", "70529192e8c617394c7e75333a28ce970d626537c8b7c7f3a1248c2ef41cddafbc440627dfa4787218eb3b6fec3602f255d1551b4125b24382f1098e4e5fb273" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
