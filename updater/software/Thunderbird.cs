﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.6.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "f345cf6f0c38d9322898110552bc249e1da757ab1e45c3a64b5819985724effad0c27673b82fed2656199cd4717888d9cf6e97ea82db7a5d1c166caac54c7895" },
                { "ar", "0da1c585d1e4dd76fec7ecd4341598eb2a365f3528281ea4058ec0b9368a0f23294168939e287ad7137374ce3a252f08d064ebda1e18e159f7eee06de127f90c" },
                { "ast", "0931d9d7964c15e165f4820851a5c80422899ba5d27110ae8187faf28e9a5ee0818b75a8874d75339b0c749c96e7d2110211164fd5fd2b571fd5a46b975f5770" },
                { "be", "5071f5225484ef4895db34dd7c3b77ed6e0c4fb6e1936769674cfb244c37fb5c06c48dbb3fe158580532fa695d5520a7cb5e716dc9e5dc039f1a6ea1d7edd3bd" },
                { "bg", "8bd3ece5df27ff9038fc35a782fadb9d6fae518178ed2eb4a87dda197692e1c29deca8c4ae3a3a6df1364881a72b3c2d2b8db4f9ad2d12d857520aa8f808ab55" },
                { "br", "51a2628e24923c422928907f31f198ca53a1a34ba205ee96533e8000a3b9a78b5175a8fc646758a78af9cbf3ace752c14a14e12d82fbc22486f3487a6b98f28d" },
                { "ca", "c0084c62d7e810eaaa0a4a27bf998d3fc363c13057706e2751b47f54e39241e084f896b7dc1b8f4ba50d574bd0f060702670cb86e03637c5139f069b28ac4708" },
                { "cak", "4f855f992d5b825336e388a2256dfdf15962ec56447f6e703711dbb090ef8676fd897a44f568093dba563f1b548320580f3530efde0695a0f6dee2e7b96e3d4e" },
                { "cs", "38ec7f2a424017e086990d5f8261d0112040b343d3113f17fac6e77f4d702757349aa26db6adf37400b89f4c40001d9c4821b4c8a6b69c303aaad609085d057e" },
                { "cy", "e45a93b519a89b79447770cd74dffadb830bbff999b0dbcb7539bff372f5a69f97821a52995781da324b48bd1ee67e79d35e1d9f16f8b656bbe9ffb9b304f53f" },
                { "da", "f9a0fa1b08ec1502092d1e25b4a8512424414a7ffe6cdc4fa0f1918c796251c1751f748e9ab808f64285a07b5e4fa71b865a68bf79c8bce0ee3120b0603c6481" },
                { "de", "3229d968b339be8ca87fdb526a8250f05d7ecf342f07367dfcbcf8b7550fb0eba3a6f85dfb26ec9eed5ecf1f92e369a0e26bf114c156c13f4d9e2f382c7e7322" },
                { "dsb", "fa6cde81b0f3ffebc0f6c5f9ada44bf6cdcf489d8e1f2d7ae18c59bac849995bd651b0d53d22c6f565ec630dd1245cbd282937ffab36ca76332c6e9a2f1504c5" },
                { "el", "34675bca415acf32374c344785ed319ebe66846d6afb62473103c1c8e38e58f53ae9b36cd92fe6c5a549da8815e198c956210baa3a9fc000bcd86871e421fa9c" },
                { "en-CA", "2b4e4bf437628d9b701962d5222a3a6e7dd10d9d0d5ece733a08ac00fbda426e529672d88e1aea24d800bf1fee1b49bd76f885f13b80f53a81ac33289d8ff41e" },
                { "en-GB", "9f073eabe0fd83ce35710a79f7a8de86e552eb10c58af6e53e9ccde8b94bd7b3e36ba1d39edabee31155bf5a67fc58114d1ee902a1c0a8df356f7ac37215c518" },
                { "en-US", "d30d2c85febd7b53fa0db351b9710d860b4f5d616167a659f7116d841be118c46f3386e2ad947a1c0aea569eaf9e52f3d3b4598c569e0620d656a50efed44001" },
                { "es-AR", "84544878ed5ee56d487f8f1abd931ce4c11037c32c7a6c040ecb91baf199a6c73a06736b2eb0e4768bc44ffc7f922dce2b1a5254cc5d2acdee1d63c0fa4fb1f5" },
                { "es-ES", "a99059dda20cbfeb99c582f0263f286a065c6891d91b6975c5c2c4f9c5bb3dddea604e6faf5e08ad96d19f70ec5c67b50f25221808b30491a1697e02895e1dcc" },
                { "es-MX", "bdfbf230ce511b9da334568796e75517e45de77161e9745cc174d670ccacded2835bf6e755674e40b81020cb66eb4e9e0f5a3a12d53042e6fa69ccbe7bc34b48" },
                { "et", "025c987497f9102f1fe3bd2ddf3fe097ccf51e6a6bbe7cdfc30673c8a084c59089a770e22b692e40c6fd9ec5698e3cc533a743baa88a17947b087fb17bb57b45" },
                { "eu", "bc5e6e5f9f7ddd5d3d4d36ee4480a33ddb0e6353fbec3151f4934f0368f39ca7deee8958914c7fb0b7791e7f0523d8ba3066cfe208668abbd6d24cc6dd1a51c0" },
                { "fi", "3a670325b15c769b1a8b2ee86ed19a937d9beb0614a73abc2d929de8e92514441be0ca05ebb6f0ef8007103a8592f1aad4dfa67f42d3819815f84603ef0b96a1" },
                { "fr", "405e0fdd3db0d634fdbb75f28a62b555d7df1550495fe4c8a71c7b4a79ac7de48b6a947196872a749d19385a0cfb3e74cafe901f1c75418220b75598354787aa" },
                { "fy-NL", "40400815c652933b18f28b41d5fdf9c3ea23da032ff5c6c8e6763ee53f2e0dde1b53a0a5cb94133a8c99ba43449d5f0fa2b916a78b00fd083b55bc4022a67104" },
                { "ga-IE", "12de43383c1cc26af96c092c1b086b23eba6c59ee85a13bbc33b598b20b0e53cdb0f5e05398ac03d1bd4f09b4ad3085abc2f5b21ce7059ba4f276ac0f6a95a36" },
                { "gd", "77fc7d181b6bbb0bb3c2a47ad53a08a816187765af41deb0d744bc86e548e623d319b1cde022855cb736c94d860c4ece02ee70748a5e69c97261554c2c20089b" },
                { "gl", "c00bfec7af73358a9479569b59ddd865dd28515b2477f1fc46c7b71c05ee303013d761c6bb4b12732d1d945a0af12ffbd838b69d0e8de26e23d45cf2d0dec07c" },
                { "he", "1d3493c79e1c4ac8f3abf9aa23c4abf6720dd736c64c72d20fcbc619bedafef2960cb145ba6c930f960db6888593780a55d9d139d78724fe2f8af2c9981ef8e4" },
                { "hr", "ab859f713afd1ee167639818ee921d9c3ea03434a4813d19e923cad7e9a331f3c46e147c9572076fc68c8242dee44075254a5132768e5b129dc65a33e52c9f92" },
                { "hsb", "edf9b2e32f5b23aa5923b867db73d9428ccb5f53c8e4999f2975e6973f7bfbae8fb86e55962e7e1ecbf7f1fb215482cfd256c38ec433f30e59acced4a59ae325" },
                { "hu", "24dacd2c48cb3eeb733e7a3469256f6d768849372078db0bcf7d2effb7e9c92a2d8ba1e590851a71412d28b930125086a4715ad9bc575d529b5806ab73e4f6d9" },
                { "hy-AM", "5c7c37f809b5579ef0a725ca2bd740662afb3073009e94b2cd1bfaf4db0866f5c73a83d160ced125893ef787ae71a148ef12e1acced177898e87add01902cccc" },
                { "id", "82ab749407da451e0636db8723e52c6ab2f0a762dff6ab5b977d385e60c6edab02ea6785982f7a4a49bdb5d3b2d0129669fc9ed364ed646b41a8eb1ef13daf24" },
                { "is", "14cee3adead5c5c8e31ae53201fd6c52d9d43f0e1c716bc443fc2b4e0d21e59c998e1bdb58e809c53da3567c84da62dba63bdf109202f78376b63a6f359351ca" },
                { "it", "e2c782b0baaaa9fa261d1b7485c44844be2f387683212386ca94c0b956f55f3c4c5488b9b607bfbb1705ec0f512b40e4e22683b76bb5ab98030b7d502f644004" },
                { "ja", "92f81dde513c0061c2bac8d3b4a2dd7b4886474a7d16573e17a82fe8b8dab7ba66838f0b325416f15514318d39009a156289d762ea89f78e4fa07ea13cdafcc7" },
                { "ka", "bd03bd9311b9ae00506b962efd4357342464a209de86d3feda00d34a05fd005b79d569fad4c093359a812ddb04f50e32b2a567814ff987e1458b2c728fc17862" },
                { "kab", "792f0c025bd1d05334d7370202083d164a65fd9e190c0391238225706334ce21bb7af7a607f79effb65b7881bb179a050702128442078fff654c2fa72cb83c4e" },
                { "kk", "7ff75bc6326eed9675b147ba4cb02d4826a712046b2c9b1d4a54f14abc43712d3d248e56f665e9681b924a23a8409f34c2292f82f4f08dc32dde4ca6f6cc4ecf" },
                { "ko", "324cc12142ec3012408e17c00ca7361fe96850dc843a9f144af1133fa6d4646a8534a01fada0b30d802e1a888b1e0fd27354a6c23960d38456ba3ea6bd4ad2ed" },
                { "lt", "df105010f0b46db3160417c8b5c7c2c69af37ba86e136ab90eade354c48d72cb1fde71d32923a8c3b142e178f8ea3ffb26ae534f5802b3a664896e9f160fb1de" },
                { "lv", "5cfa56ece67ad67c585303b9a986533f873124394b69852dbb876158cfe1996b2ef5bc860acd053b7df8bb9a4193a51e062980706d058a11a9e30ff23e443b2f" },
                { "ms", "f5a75529a6349348d6621b360b48e16b2abb4c74d510678b4f027008cbeda209f0b0917e8a5bb012f51416f42d7c6de2b0cc3e7b129283887db4c87ec5814063" },
                { "nb-NO", "31b685b0585695f0730c81f6a531784408aae3631a3074ed14329b04fe3fd4de836af338566df1f9d1d461b9d1fa401a216b2a79efdd57288f76487d94056b9a" },
                { "nl", "357397953fa42bf89df9e7ea21fab943cfa2529b1b94937449c7762b2f82ac2b99f337ad3a7dea0524a3bc5da959e46dad5a84b1dc306d3ceb594e306aa2d29c" },
                { "nn-NO", "908380e09dc8af646b99e91d0cc3c43713d4ea939e806c6903a8e7366f00e06e18cba09120967f9d868dc912313929681ce44ff536ea3c966524a8f77f6b77f9" },
                { "pa-IN", "ec540eaa55e8c9b83192705d2062b3278f5ecd8e9896059096cdd614a10c5df823181a81700fb5deb5bd8143b15b610ff40d4a1d013caeea3ff3edf9a8aa4582" },
                { "pl", "4984a104644ed408f1d6f978e7bc51a225e74f4b59180a7b4cf1a938ac014fb0f5e3b1c709feb4a5f626eb3a8ffcfed33eedefd9993071b093cbc503f26d6bad" },
                { "pt-BR", "e13698882c4af5af3fab22cdb725b9818df95500898956da9c1ac2691a45cd09d8e15d7000163a25ac0eb2a7d53b613d5f77c3c6abc4e70081c0ff5a1b783661" },
                { "pt-PT", "3309466b54884e37c0816a1c9de3ad29ba5d50a02b2e239a779e278fd508d40928bb7d93e0639229a4a8331008ec89e7a595d45ab18df31405f7fd5a3f81b63e" },
                { "rm", "27c04784b4dda7cf225e94dd4ab0a160fc0490120d6bceb8b2024543c4dffa3ded4547fae8c86c72dbd8b78ae17475a93301c769b8f453c281c16ff25b7e2d86" },
                { "ro", "cae6db8aead8268736a1c3565edf8395281ff284e575b3ed182d4f8ca65b7e66ea2d6ec887b2d0e9cf84f7be0321a0def26ac3ff271094c082be5ad6a4f19e20" },
                { "ru", "a555b8bd6bde960a183a5d572ce1de871482483194c0f03a72196e18f3a1aa68c7ea7acc25a6a2afe3060838c3a5a7f5e066505372159ef7c2d031428715963c" },
                { "sk", "776fd8eec0236d7e47d982815d833fe42f5dda1c420177c5b2f7fbf8bbdc701ede7b52568f943b45b6ec89335f34d9182dd258e7e2d782bb9b81b05775c81a2c" },
                { "sl", "5e0c4058a4399939503e6104c05fbed308c96208e3d4079b98c5f6d17f26982acd10aa1d1e271ab019ff09eeb4ac5562eb9006ecc053f3c0bf7486ebb6d79bf2" },
                { "sq", "20a90bcb9c6df690030ee39be8a2a8f0dbe74a3d651c18f8efb2e295585db12fb6593ded38b92a1e725bcd9858c26ecb9473aa493c10df7874701915fe4a2e57" },
                { "sr", "6708883665fb4eda22f01c4c8b9ffaa6a0f776416c2893ff629d4b9db0195fc1620a68b4135110fd8eccbe0aa72e1e2ca23e842947eeb566f4891d1908bb2011" },
                { "sv-SE", "3bbd9ce665e87330034fbf459752bf7c4da2889e603662dd75e2c89966234734cd92bfc52c0a4c06eb52cc22ad387f74946ff948f3a837bd18ffbffda950e453" },
                { "th", "8dfdd7ed8c5f5226cc7562dc6d4dd414df226344b59323f8640ec6935b7d1ca4ded1bbf684240eb1774d04c7c12d57226ba431654d663033726fda15b353a9c8" },
                { "tr", "e2a84038cef52211ed580d8dcdddd531ef9ce8674c92e50ecb4b015d7958b44b4f6cdc93182c16b8d468426cd6d368fdf00e171d906da386bae1b2013172d7ae" },
                { "uk", "be42756af046f41fc707042b37256b5032b95ab34470e1288dfead672cdc283ef705b2980ed966e428049b77ba45c0def949b0053941d8ed4ab4ea1cfff92c4b" },
                { "uz", "76f07d97435af3a8cae6ec3e65d792e7a08221924da4b31ca724a93b0c922d4d91f474fb7bf34b1e785ae6db1aae5135f0df7cc2a7e339562dceae9c1a1b0de0" },
                { "vi", "c6e8e908c7392fdfc8f82fecb03d062d4c05cce5c85d27dd9b8056ad1a92bd1b6fbda312e9942afba3a0eada37542b557be5af9d25ec50a05af051cbf92f4bf5" },
                { "zh-CN", "f125786d47712a07744610423d15b432f7e103232792705c2d8c812f1a7561f8c269b06008f754d73eb613d5056f720502accce78bd8b5af9a5a4e7c8bb2410e" },
                { "zh-TW", "3055d5eb8befb2bade221f1906635315ba6ee09b29abcc78f767567bf535dd5cd0af652b5ff8d15b14950d8b34fd6b5281f5d09f1ebfc86f0f5124b7f27a08eb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.6.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "66c19c487a73b14ebbfcfbbe277c84544aee15bf828649fdf1cd0e95d6eccf0232e0d1f0b660f03053c19fead5578cac9e93ce79487b4daa4ba0bbf5d3406f65" },
                { "ar", "4653c118d72c7b10dc41d63c203e9715ebbb28597f432304b851c783ce702d4da840af3ce19be3a4854e2a68e57649a20a4686451a2ba99de901a2abfe35c0fb" },
                { "ast", "3a98832020c14a8f1f929612629d83f19f32e415d6c21d1987422fb74bd7f84f9a24cf18ff7a617a0c2b693ed486b410500a1e4269d4c95aa72e9e3bec312fcc" },
                { "be", "3f211a4a84b34a7b9aa3dd512d65521330e6c2522dd3b7285d791e2a68160b88c770faf6916fcb8169b453a5f33e16e648165f0ea4de0cd24865d1fb607d579c" },
                { "bg", "7470ed02337e2ac35c7cba010c00325d7238c524040b19e745f438699ff555f55de7e23217c76a63ade5710df02682b936718439d245f39b02aa5075008a8709" },
                { "br", "da9e9c4d2d1b51b3d93b0141ef1249179965f29d386959a8b265e8686df0b6af47a5ac39b9c86152ec76a5420b01c0b95e5c4e05a6a2c2a99fc19fa801a16715" },
                { "ca", "4ca11ef00b54cc72d3e99d4aee938049dacb58deebbf3306369f2f0d8ceaaf7e2888ff8e969327c96ad6079b2234babb36446b52b2b25a5367d03e3951db8f80" },
                { "cak", "349496cba68d7d9ac5c085034236116fb1c68db84c619b1bc837dbfbdb79283aea1578245bd40ee4923a0c268265d52a676d4800b9734abdeca44476fd8b9f17" },
                { "cs", "a2904e50034dae6e2cddaf3c79e22cb672281025616326541f469359ac2ef0b6f1050e4e2bdf7541b5d330dadd91042ac81a4dd28ef155489090f4c61700d39b" },
                { "cy", "f6953ee578af8d1638b5508184ecca27b9896b6feabf1ea9d15403d9dc8b4dc803ae6ba07fbdefdec3a2761d83e57e209d3aab7a41f58ef4f5c9150727f99e4e" },
                { "da", "bec7f5e55e022cdc8e282542d7e0fa1513aaa9645b441f1eae315a7e74978817e964c03240a88aa0a7f86919b48f5e1282685ade0f589f90eb2a995b31dbb717" },
                { "de", "4c3c3784c93336a49f57788b66bc2bda74ef66b9d46fed446ce9321b543f3995bfd22a703f61a1d813f2d74a2dcd706d31472c394a6074eee3276b2ec83f600e" },
                { "dsb", "4c7dd5ae8ddb5cbbf494137d5c16cdbfb5f6e219feaa2b435796cfea77e461c9921434a4b197a9364513b9bcb35f0bed9d4cb0e92da1ba39c878f1a77ad1bbd0" },
                { "el", "e2c2d435bc9b050cbdb1b4dc20e43830859fc15e802ec1ea72a437da8f31194fe67691262449dc6e47e8f390291ef4f582830f2bd06e11ef1e186a65c857ad09" },
                { "en-CA", "468500467eef5ecde17068826778ffd2deb205dfb0cc3c8c3b3a194d671c1e30442d6a7e73bcf7e6522ac04d024a90621c0c258c56a1e0f5de34cd5fc35e971d" },
                { "en-GB", "5a83c6b7b42d791392478efdccd793eaeee4cff504058962ad440c68c0374e81e856a3412e9e350cc60bcbd9ad1b4273f0eb5a52995dba2a9625665a14b0f6d3" },
                { "en-US", "b90c77f1d96d259d145268a0570bc9a7f54557e18ddad68b0f718503435d9f4ffa829c8430389a7d02230768c6a013ada7a6024239687b36e8b6e78f225ba177" },
                { "es-AR", "6dea78c276a75040f5c4c9dbfbc8576ef45890f3ee4738fe6a712906b9b46e083d93696b7131e974c1f017ebce4a525073538fab6362880a91e0852fff2f9fc4" },
                { "es-ES", "30b0bd428a639d0e84eda6bcd4ac94e0587cb23cc7748c98888d7b11e578303c941fc1a57581c1712c46de4a2628f01a7cab9fc0c2b97bb20889f192a26cd683" },
                { "es-MX", "a76d3e0bd90ae5cd7e7cbc6d93a6f4b9e47a75fd458527e823a494e44f4384cd1723183319485a3c43eadeb98fe74e15118cf90b89f87245f1d8252fff0598b8" },
                { "et", "258c23ed46885677f2fafd8c5447c13e1da01f130daeca345eb3bbb2ac4843f00793c1d4de90aecb847829d7e954a37d37d32d54e1e33c12b2c85ffeaae02848" },
                { "eu", "117eeb9292aca1b21ae85c0ef073bc7d86fdbc572975a48073237cb35cd300df2f095e792f800c41bd35224d6d4d48a5dc9dbd2602073d82873bf3e5de46bb73" },
                { "fi", "61be6887230ce7754675d3007543a4377a3c1e3f098bfbb2da03cf685c479cacdbbf0f6426e84bad11cb6c7ac90b0e759eefbdc2e61e015e693a7311e4ba4628" },
                { "fr", "7428539b36d0318deb625d8881a4ab958d2e901eb4be963ebd44bf8700e6c2d00b702243e1b87909e17fbe2cf0d62a3242d9772e11b56aaa9f90bdcfbafd1ebe" },
                { "fy-NL", "93fd8b4210a0a2af7ea8be13179ff26dd36a66b1eeb44270e0f6132d2804dea6db4202c2c7e41f9adcf66a6f16be942106ad144a8738672a9ea1b7aa8a28ec66" },
                { "ga-IE", "3c0c957d9fe70fe8f5a970ab2c51c310edcda1da7056db915fa30c07213f9afb2513bdfd5b5a1c0d2f5c5ffef8d56e435dc629b0d18c4575a183c5fdd7c5d427" },
                { "gd", "84e42bd004b57a12c32e14c1b22c6dd4c9812026daa70a53dcd6016b8c92a5a4583a7cd1580ff73b6aa3c3107b504a63d40f2e597c869a1335f762f871693607" },
                { "gl", "dfe2852433531df76e0851d51e9efe07e725b7420fcf053d55370a7f62f21467acce6f01d6c82760f595c2c1053bc6ab477808f81bf224eeb522cbbccf5bbe93" },
                { "he", "0980b15495b06beeaaf63a4fba24246137687b3e069cc3dcaf10f328b4c080d9fda170d9dd74abb646ce9af862bc0b42cbb9e59fbbd0ca5439c3214afad70084" },
                { "hr", "c82e2433a7c51228d9faf32dcf139d260681926725af8af3b24599a05f5bed16f6251d061d622ba00392c546a6cb357ac45f0d434dca8069513c9112bdd761cf" },
                { "hsb", "8f72fee3e42a0bd749bbb3f4560a94d8ea2de0f5daac3bb41768d85bc73de991b49364dbe029d2fb54bba0542d7b99952db867fd36729d1360f9e569c6b3a217" },
                { "hu", "81979ba977844b59c7470ad0198dc2c48266e8097d6a26948c93dde2de2e250ce7018cd2c261b3ff4f3cd1271a72ed2eea799cd54fa691533be0a06022f37790" },
                { "hy-AM", "bf6832d586253e04af03c5c85307937abed36764c94a924b2fb038d0d645829fe624214523e57cf5d0e49b97178cd3b8b1a83f86e30eae2d651c0d7f59528be4" },
                { "id", "ea96852c21f617ba2b70c0ce46fd51c929e342143e36a1a56dd9d6cfc6d86d84aea2f92385c428c4c9f6ca18638601843f2ebc0c74712754ecb91226470e6ab9" },
                { "is", "f1e3206c82581db392229f3430987ce0443d0da641498bb8e70aa3679bfaa77db8930ddd908c14462e8f0b689efba0acbcc5955d896323e1fc42ef316536a25b" },
                { "it", "7aee998b217fe42aba143aee01aedc6b859df2a92bbb91d6ee21506d5d7c3fa54b14e230283f85a614f997c0d30915d732d23bafea2cdfd7055d13063adb0e87" },
                { "ja", "1bf64d086119d625d21c8e07c4fcb4ef40af95ebc13706096ced9e2c84fbb9d81643e789c2a9d4edee6ab6d976fbd21b80de69fc561c872039aad011cb60ee21" },
                { "ka", "1b342739c746dfcf40b666236ab2e84d06609741b254dd0e49c847c77b92a95022051d6dd7370e16ede214a9e8768ffa593e1710d41556acf44338ad9153bc37" },
                { "kab", "4c4655ffe1c9ff6bfa0127b33472797ec4b1646d181146ecc3dbfd76dc4df43c472dc41304b01d2847f3799488fe14325e53b965d2ced62e0cfd70d2f228ec1d" },
                { "kk", "bc1560333f7e520da1442e843617aec31486e760f53142d1d1dd8e0c8cf07a5f63a899e63e1b247e77ba922d04c24b55f4edf5df882be62504585390e40d976e" },
                { "ko", "b236c35b39350e5ff5d9e425f9626676d0c42dbd47f5f6b7b0a20ce5f9d42f54b5467f1cab66e78a7525c9385e7ca7e94819dc4f07211f1f3ae89ecfe697a5da" },
                { "lt", "4de268624432abaef8242af7cd4b2b6e68ef296f025b740dda292076d8da92a7b9190321b4b3dd9b513494c00ed3ad2eb4090cf2c98286ad4d08f7cff098b412" },
                { "lv", "9b72bb3b5440fd4bc21ef7d859423066fabd27a69e96685eebef151e4dea61d211437d380bbb7f540cdabeb8dbabd39d9e589c0e0327405abfe99bc9f96516bb" },
                { "ms", "272b03c3f488267d78cac80c2fb8a02f0b6a14cff07f7b52ea9860107b2ca4a39c59a5b10a3b478d6cebbada46e6f56524893969a49586ca89de633d0541102c" },
                { "nb-NO", "c367b7ed78701b43be881731c37fd78c0033f1dc70dcae0c1e4f8882277b0754a99cfc3c4cdaa49eae70afb868f5ff178c5115477dfa3b6eac775323516a6a29" },
                { "nl", "363a00ffcc18ba3dbb94f3d07d1fa7c56c6c60222d3d786379801de08addbdb44d3f2aa72f5a6839005cbb5ee8714d232e4e88d5564c92a3d9f9307f3c23783c" },
                { "nn-NO", "90c1e9258e8ad0b5d5531e606ecc57d1230f2dbf6e8ddb5d019f25f409b33571e03f3353504c27bce370747d7800c7e11e6c049e5e984d693fe9b1c01dc74e91" },
                { "pa-IN", "a0c52a5710b487eb9a855b31b119ec828f619d291324fa6200ee1a95d3d4b17739bef1b8e6175902c15ecb43f61e56285fe59f2bdfeecce1c325971b85f392e9" },
                { "pl", "f12d71af6666ac5cf88ed4c6fa0e6b700368cf5ff874e80e91b6c26b5f560f23592154927a66b5fe9f035a215a1603a8259dfab2a6fd672f4c68c3d61e5be926" },
                { "pt-BR", "d051f9e05b5209b388c2a04ac4f1d4df90f11b2fcab10f3c60c7d627a32025b3acb6ece804885a27e35a0f97784f13735e81a5eb318875880eb95a37b7fdffbf" },
                { "pt-PT", "fc49163fb2b846ac9954cd62dc7a137b1d2b0da8b6e37a6b35c7c735ad52d2f97c428dd4b0e4e5c11667eadc8a805bf059d8cbe34d7fd155cf2430deee36ec27" },
                { "rm", "74557e77ebe0fc0c33fa90c40b75b83fb4c4c0671c27482efe847f034b5e66a9692955ea119809cc2ec0490947b185c62a863778966ac0ff42dad4c515495d0d" },
                { "ro", "7066d396bdb9cb492e743ed5c906ae50ec1daf04ceec2f02861396b6dbde61974139152a6e073f4e10fed17863e3bfac6861ee6c3dfadca22b629b81d41d8079" },
                { "ru", "2a2dcce6968e354a91e8de6754e21c39e8d264a94a530e2b4fdbcaa064876020cc5ade26903cfb6751c8d760be45d78cf373d77747fc34b9d38b5a5d6e5478fd" },
                { "sk", "d6885fb5d045a47e9dfda3825a706acc614108a62e9ce6436953490606f607094f19932edaae2cdf1b2118187e4c44f5ef7eb7e0147dc2d8527a625dd7e48e1e" },
                { "sl", "962615605d1d553060d9a52d1e5d719b9d5a36bd39abf990cb6b272a41fd2c102a7e708e6efbf119407834155b00887897d898f820520c64b38b7c906a5f45d3" },
                { "sq", "7143ac051832397cde21d26c8d7fb8d4fa5ffacdd54e2657516c35f781b44b7f0d12e6e742a42839f9a47a6cc847f273a17fd417194d81907b0b0d9633f22432" },
                { "sr", "cb8684a2124cd09cb05333fac36c49becaf73bb783c7bb1155dfae286f7e77d27a7cc50a4aa72a09d4869fa246cfa453c5ee81bc668e9e6704316fffde2e86d0" },
                { "sv-SE", "280f14e0e24dea5e9ebcb9f24b72703303d136d0a7699c9c6fc565329de22577ebca2b0850b905d5acf593034ed5dc3173b83e32724ab54212c03e77ae118b24" },
                { "th", "c7f0d8a8597472fa5541dbe088e5110f897ed8a56a4d2e5396643f480402037fbea86ae5e1c64d7019bd22764c940c7947b67675d24dd3bb859591f29cc58792" },
                { "tr", "033ea83e66511e2725afed15042cfad497f127330baa6fa8d1110a6fedbd0c93d785ad6212f7f3ccce0ec76801a0ca25aef8f414448fdc07dec9d63b209183cf" },
                { "uk", "5351810b90f212c7714c07808f86247bc9831ee18bbbe450b1cca4170fe49c53428a1aac86529c3ba2de2295739b9aae5469ac6ed4b942fe553b9b992a805c5b" },
                { "uz", "2e7f2b012c2bcbe6849c5bf1687992566e3ba84bb4cb67c27d3de40df862c11efe88e05b578c1ce924f279d8f244ad53f3152450cffb0ed385ca6afa4d029caf" },
                { "vi", "b822af3b58d749029c831b1e036dc55fc8032b9877a3fd57d323845e72a584494e227e108be9db3f0b3865d51c6634413db54a526f4877a81eef912e423d352b" },
                { "zh-CN", "150d4dad0e34cc8cd293d8498fc9aef3ab4b34ae8e97f219a09012cafce2ebdcdde4eb621178130ff8f94bf101742a72ec04e2568d4ce95bb3649c78836882a8" },
                { "zh-TW", "ff261b0d0c98e62357d16b6ad8d6935de04813dd5891e46e2b87bdaa8bd8c6ab5719db024d9233c5fed424cd8658410d99f89cdbe901660ec707a9f2ca89afb4" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.6.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
    } // class
} // namespace
