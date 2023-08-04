﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "117.0b3";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "6d8a5ed3e15294a9689a13bbc4426253057ef037e0cd563e7c42400bde8041a75f691aa768dd673353d23383445b204ce00925c3da657c2ddada1856e519a9a1" },
                { "af", "501447c166a712daa5a6cdcf652d13f2bef9ec58c4dcafb2d7bf3a5e0778cd16196809d71ededb36609f2752d2ccb43f8bfacdfe16c7b659aff5c827b5fb3db5" },
                { "an", "64d76e03daf60f4aabba637839b5ae9f78dc473880bc3e8d448c34f41091b44012513a02f2126492e4eedd5af0ab44886df34ede194e1eebb67a9361529e1d76" },
                { "ar", "cb94f92b568aba557acc183fb51f7cc9c79f7bb42f71537ca7a9453de9c666b09ed6ae01c7ef31bfea6d9399b1bd5aecca0bfcc12da27bebd3017b8707e34cb2" },
                { "ast", "625961247ca4e1a97cfcd42ce3faf889ebc43f74ba5f5f0d6b336ce9d154c2bc41df3d99e28e7bea8ade04efec4bcc0191b937abf39f16310a41b7d0b0065d03" },
                { "az", "547718017a4090d743cf9b560f544e72225d675d8fe527ed93153833b81d457d5a215e6c2496a47b3599023c3e6963d53cc22bd31e7bf4477e381df44979ed62" },
                { "be", "3dcd54567e36564d03e0ab489b599defc9bdb28df90ed7a720721c419477ae22f72f07d322e8360e4f0161257de18850f0ee79556e89289be925d44a30955425" },
                { "bg", "85dadc0072dddbb729044962c99c8adc40ad4f351fa49e993590b35c305ea93d69ec0d1e6d6e78fab4a3163f4767e3c92078055fdeeb572a3ef131f737b50496" },
                { "bn", "9b55565868757f1fe02d0a25d1b2455be13e6fd1c187d9a0da01b4f798fa30af5c498515e51e8e708fd316612d33d6c267b44a87b2d7262f9a20c6c2f242d669" },
                { "br", "2af95fe01c884c75ad7d14fa15732e22cef0fc3580154c6a3f1307221e8eafd423b70d1671d8563b06084c336e41352f3b51a8af979aef447159a68802eaa9e3" },
                { "bs", "1734bc5f897e9668997ae6f550bd77522da63afcdbbc7c4e5f4cb51220530af7deb5350a375371d6c674cf4ecd7719745e064d75034233d35bce65bc855fa4d3" },
                { "ca", "97887e4e527e6898ad168c7108e9b23b75eea5aace245b92e11c3ff72fb26c30f1e54ffa73fb7a95536b3a62ae3fc5b18abc64a0fd06d91d09ebaac03c3ca417" },
                { "cak", "c306514a367b5989cf5810e07cad751207a212970d1bfc11413ff46318ed6887855d3f9af4273f35b597428daf64480f726ad80ed33325ba24569578f3f9312f" },
                { "cs", "7e1b3cea5843a253381d100b489d01c70daad7d014319bd08dddde026c0d4be462ac1a284b97de214e4171774a301d9bc84f6ff6a553547078b8a5fb85292c40" },
                { "cy", "b0b23b08ec4b0175bf7d4d9f0270fbf0df2d688838b9d374d66f66bf73e8e49c8766809b5c4a06d6c2fdf73f72c9639fb399f95e649a532af0c721e338fa0d29" },
                { "da", "899fd98aa075069ee7d5058f4df2b5a233f87a7256ea02d6e4181e9db2dbd4fa500236531debab95a1fac4764bc6e9e68daed5a59142827a08912de4b321789d" },
                { "de", "1d897a339261a82ea7924bd7f05c97b686c2f26affe8a52a80d779a99e3a77f863500617c5f649723f5a36203cc03bcb3353bf125f36a89d5383d21779828533" },
                { "dsb", "50c594981a57a1e95336f5e8337f9e867ec6577619fc1cfb8adf1efa3358d7ca46489e810620df39de9399cdb91c0ff4e46eeece76e74c9f7b19dc44b1378861" },
                { "el", "355845fe6f9b8fb96a7a4015a669188b559534e941fa59c3e5e048ac6f01fa8ebe349808e576ffd08355fe2453c05bfb1b104fd902b8c746de4961dab202c265" },
                { "en-CA", "c993cf8b74c4efae110bbfe38c7628613c63058af5c1ae610dd7f246a87def9f1fc71647f3cd190c23e17c25f0ba1d5bab161eb6ae528e216bf73bf0923c1960" },
                { "en-GB", "a35360f391c1994b16db43837e4f17e0a25eac03d362f1f56b569ccab3a4612052fb431fbc13032a50cf19dba54e7888d2b8b91a4271135e190189c965f7b439" },
                { "en-US", "0e51fb3de4c6c9bdeb0a00b72cb64059b1414363b0589ba8023e1fefe1cba6de9a92d2fb10651ebc0e57f5bf7e6f5033a7db517f5beca39b33309d4e31c194b6" },
                { "eo", "cdbd1ea047e230afea7b9e61555f14d73433e79fb55ee280d483286dd3decd222a230190591694c265307f02094ccae5511863b1a326561cbdd314ba94fccfce" },
                { "es-AR", "053b15b6330725f59b7434bb4881be9e041dc09f6eccd16b58a09d678de75ea4a17f18adb71cd419cd4ab4ea290fd7e08e894cd756954e77890e9b4b29b557c0" },
                { "es-CL", "ceacaa528265c760d98f4d6958197df195a77e49f55be762671dc74866397db7860c682be7f7659bf62ba5c8ecc4c4086498c4aea5a11fb096ff552d8644f580" },
                { "es-ES", "7c9f67fbabaaf24bba82e5bb5d9401c8daf4945f0cccd9029e0694030ad3246ca28c18f10413c532f2ebbafec367f779774009c41a19b9ae18238fd318a3df82" },
                { "es-MX", "6ca8c1fdb5bc18fa6b84a2613007afede33dcfd180074de1ae86f9f2fa10507cd43492e116c4f96aa218a3436290b7198e4c3fe924461b39222b2eed14d588c6" },
                { "et", "91de4acd498e5f0dcd7ca28161c51173055c3be843a49c1bc87a3d16f936a3e185a98a78a9f498d2698efa5fc62180686c51fe13e12d9061df7d50add0644162" },
                { "eu", "e4f04ad52dcc4fe94269bbd60395360da1b3bf5c2ac31159f096fff9af8802fd5721eea2df98d5b18470cdd1b3725b95190cedabad2cf68437ae8e9bc4c75716" },
                { "fa", "e38bdf5322da913646e8a3026412449fe8605eb9ffda067199c9acd0557f8263056553119cf306aef7067e99a7c4d8f60c395f7b7f9ef688cdec113fdef528ed" },
                { "ff", "4196e48e0d1e0be8c14e33952a53f2f58e612f9e26e42b01545ce8671793a1f32af8770e1ebe281f6df9277204777e6224a3cc555581e7c944113305d2d1cbf7" },
                { "fi", "81a3e22780f1dc57815f2fe5b9a22cede6d42a7336b7ae80003637ffd0e8d88e260d48246474b65a6554880829e4fd454cc5c757c8159294404968c8676999f8" },
                { "fr", "75e80167e5477e6076b575f8306a3d90467460cd24a83e6e6be45f88c376bcb14b949fb902d9518f6bbd2bbc6822c110dd43e3d46cebe144117d3c87ad746cfb" },
                { "fur", "b0380e2d2931cd1f8dab8faf75514c113ea6b642c7012d66269bf06973d9b8cd1cff24be405315cb98d7fcb4bb179f1291738a8b220b531c75ca783c3ae2c3cf" },
                { "fy-NL", "bf7dd43883044d5946bd0cb39183776357789c76aa2690dd7a7934c73ae2d19b3298acc1024a41188bc9b45e6a169393fc250528ff18900e8bcec25a606f7413" },
                { "ga-IE", "17b7cdcc9385d46f5968f08a2868447bd19eb60bdfdba2acc6b5963032ebc55c60b40748ccaa91322ad4ccc03c6c26734347b18dce65d26ad66ad1f49fa26801" },
                { "gd", "7add6aba124b02c656fc8b91751fc721a5f7c43868f1ebc620493313e7dba9d91c43d79a6071c5e589599a54ca83c7b26348efbed5bd12be25cfa2ea3cb38d48" },
                { "gl", "e2049e88feb5e2b0d6bd42eea4040f54d9c2354e985fd7a6ebed9f0bdddd1ce8daf61ecdb0a29bf1abe9d349c9df2d3ba6ceaa5dd0896e902f97aebfaf74dc0c" },
                { "gn", "4564b2cd3509d91293449d26314f9a162d89cd36ed28a896a1a7c2258c4a52d8bc993071d13b31827753e01db46ffb73c4c3b95e0efc708afc67153d9a020b87" },
                { "gu-IN", "f6991d5783c897c4da75d116ed334e1fc0733c20a0c3d998449ef50186775753dbb1b35c095ba1fcad27f3f50035b5a0292516a0b601653f5b033d82b5b2708b" },
                { "he", "2da53828dec61bf8eb26d82a2aba354f821565c4d80145f49a5440d46bc756a6eda134c09461950fe7d381aa6302dc10e02611f1e5c1cdc5976ea4e1590fc448" },
                { "hi-IN", "5cda4484e489a859c01d4e95e6825dcbd99cd8224a311219bb90b72deb8daa4267db82d191be106e4f7e3789bdebca2784bdaff3b1c7d5b0f31e340b0490c55c" },
                { "hr", "b7cbf155d16f5caf599fdf376227388c2330ebe19a2ff8c645a4bc6c5253aac9b186b8dd620eaa0af41ac35d062e2cbbf71e05e52b14cb2a710986d16236444f" },
                { "hsb", "d96438fa217658cc05cef15ba8c0768ad2341d803851395402da799a6e41d717990a02f78c704412ce5e047f212fab1adfd12ed83122b08b549a2a11c05f63fb" },
                { "hu", "1a47e2e2a7bd537b3d068be9c5a48d0438ba7e7dadae29974c02cc71c88f554e429d92fa7f268d09e4fdf1d2797b01dcac1fc0c1127c0143cfe99238c7ab73be" },
                { "hy-AM", "1256569de5952f148fdffe852ba8f2365f8a1588ad8d963d7689e13920a882dc3b817a1e8f4c5d90254ccbd7f3dac924977a37dcbfb204eee9825599cad8abdf" },
                { "ia", "1d6754ff0f6add270bc6e03e5866a21908b85ccacb304952b92fac345e599c24e85c5ab9fa1fc327cfb3f426a826dd0aaeb089bf30cf0c5af3f7cef74a27054b" },
                { "id", "09c352cd65e44348a978660c1bea9e3e5accf0308c4361bcdffa0ae814904d9489b86ebe0b954f17dfe8ca63d699505d3d09446b93d4fe3906377dfc8444be55" },
                { "is", "d046edddbe15fe88f951c9ea52ba6558e8f4607e1782c9f0450e8e1ff14eda754a9fc6ebe38679112c6c080fea2bbe77bd93213cee8c7157a1cbd55702637e5c" },
                { "it", "c14396b0525d0656f505195b9971ab8c37eb5bcd07c3953ff0f89e21f1f3a7702f00486caeb25587de4cc4f7aa850426cf7e8f289a69a22548d2d17d6979dd32" },
                { "ja", "4491a6ed7a3ff7b7aaa69ce7d71c86889e4876ea1e9d436f9bb0691e396d6121881a5b453dd491929a29445b264035a06d4b967d9375e2a8336e6f6a1feae1ac" },
                { "ka", "e72501d6d03eeed446c8172b80e9ee3afa30b6ac3704069bf6fcbe5e5e9c42c11d4aa82a0f9f4bf8a6023d9784dd69b5d765becf49a56f008b25a2c082903295" },
                { "kab", "bb7905433d21a5c4b66a1e4638fbb4751e2cb4f191dcf3a4ee9a43ee0941cee3c12de8caaee79b6eb2156f9617c47fd46ff888b8bad9136df1dba993177a9ab5" },
                { "kk", "005953da733bdb6a972dbd20c145b8093f2dab1e7353b022d442d65dda83a434485041371ce5a4bef440922ac3932a3cecc206d3aaf34591ae30ec0295564130" },
                { "km", "ea357fbf5c649a1a822b8c79f811dbbfed42a24afe226b5a54bb6f5ff3f78117afe48aa33d4ddef6625c4d9000bdc81ffcad2536c84b6b95f00d3e8928579778" },
                { "kn", "5dc20ac97f03415ffd340a3a2a02468bf73199706d7fe571a9bfa7445564a6ab9bb596dce652fc60ce9ed7ff2acbab2ab84e087ad4553969a1f9c22f249b1619" },
                { "ko", "e437cb8fb342fa9dcc0af6ded013f4a5ecc6508f3f27d913a281e74003d66559d69c73016466c66bf546e1c2d71baf69ac3e88ee17e3380f1f9c1a228d7484de" },
                { "lij", "b89e6cd821f44f6689d147820e945dddd1acafe4e155f67f810d9fd7b5da43e0e60b9aabc6a8b0344e27ad599b51c5ea552130e1665c8317e9442a60301c65b9" },
                { "lt", "47f95b90b8ec80df75a405ddbc77e19840d971b2230ae2905d2adcda4d8ea1fbc1e56e383a10330c718dbd13e9f40f22f8895476c984ec81e68a2a489c089ea0" },
                { "lv", "fa14d731e86c2d28376a7e1920517ad269bd3a20d9f28d5ff226e23c1902fb7060ccfe8e97796bdad56d6f0f2736db1a953bd2c547d5450e9ea9d474214367d6" },
                { "mk", "4b3f88028158c7b5fadc17ea6ebe737283df5c948d67d4fdb66d3e0efbd4e45014fb2e324c1c23af753e53732a2db2f3e956f2837d96c01ce00f6c4917d38e2a" },
                { "mr", "60d71d2b6e9773d84caa117a14e7691f8805b7f5551a61ed9ee24ec2cbd5b1fa7d0ebdd033e496c72d93b00f871514d74a927d81c864a74cb3e5fab8470cf1e6" },
                { "ms", "8bafd835ef9a30f4dc5d9335488f06a1dfad76f93cdb76cbd3ace717be0c4b9a1d8d9f1209e9b4fdd55efdf2af9f79be78d7a4fb004131a30ccbb6c559bf0bcc" },
                { "my", "f4091923c6465a3fee6cc869820df7462ea2dbfe1dbc5a319d5fe244ba29240286538f149046ac246ddd4c0fa0ffe5d7e268236f63529c1c69176a9eff7352fe" },
                { "nb-NO", "2f86a56665554a27906016d20908d5e0f2205e712b68d1c2a25fae03f5f50fdfa6ac0a3deabccec67998c23c8bc3b8a0138027a511e2e124235dfd674b7eae05" },
                { "ne-NP", "845ba7aed91aa77264a024071d3c4f2d854a92062114d009e22e15fa6eff16d0d4336ca775e6c5bfe1083456444e329b76f829cf339cdb52c02a6ba20281c0fd" },
                { "nl", "dec270d5591fd221f796b081faf3a817ddef41502157e61aa05fe9195c3eb66816243136f74b4edd0765c6fc9cf0930238ab5b1a275b2893932b962c0cca6095" },
                { "nn-NO", "29841cd73ba8cd2a3db19a686b387db937339b1f1a99aa6618044f15a0339c182b02a56bdb00a9a4b358519028a21ef14cfd997fc7c6ee74e88c8c60806b19fa" },
                { "oc", "96a2da52d7c874a7daaea4b52107861045be4dd437e70e9f95f9ebb92ae3687a54643a01605f0da9a450d27ad264b22e19ef291d911654bc8da7c0d2604bf9fa" },
                { "pa-IN", "72704cb4fdcbff910e965ebae452c71592b93da5a1e1ef778000a0abdafc4b6b5d6f08df7af5a9a212803df188745bd2a1991dad988ef13e9c31d505108e4dfc" },
                { "pl", "8147d7dcacde91269de9a826edb19a23f468cab316e330605f2fb593392f0039aa6bd2692d70ee0ecd538156020e970faf138b4b115e089d8b7e5dc8fa3b57df" },
                { "pt-BR", "8540653c73256cc0614c09534e925008387f18d01c1c5ae003ebed08eb696762317459f143e0fced6130011ac182966bc4f91b04fb7b114ce0c176f46c5113f0" },
                { "pt-PT", "c8058f92ab3fce9e5207a2c13b526f66c9dca01230a141f9eec297b052f4f93f6bf6b8a98ee0b2c54700713a383a92c212b08e9b14a34f6a9891dae4cdaa50ac" },
                { "rm", "cef8e3045e87ff00b2eef384e7e6aceaa66f4df7f583c8a650f2030675315580d7ca065ab692b72d03dd576ecd755c31ec427d2c9c611631872d9e67e7762d02" },
                { "ro", "ca48d88f0e7b1b459e55cb1d7b4bb5f1ed1d6fe8a504b9fdec105b72e824d89120134e66f0e7bb76c2ef343b1d09eedd2610bf5c5328a97f3512278d675d5910" },
                { "ru", "519a51fde86565d75a47334c563f933d5ed6a499ba543593f31a9d88754e9c45789a161d09df00f9c5ae2b2a27b558183422bce74afa378193219017dccd504a" },
                { "sc", "40a7d01a871aba80724d0580c51e4b711f72160444fb2fc46a60727a6e89be6aea94e3953ac78ba060b8bcbaef8f0df110beec5d8242ead1a4d5d9187961f8ad" },
                { "sco", "cfa01f5d4d4021dee45d87785dc7bb7886e76c0423ee2410d975d8b9dae1d360343f8c2fcea6d48ec0a56bd28bd8092849aee93a6f74b4d93818bb58ea0ae98d" },
                { "si", "1fd7ec3f86e6c3b87206bdb689487d40e88a316e40f23487bebc6fca72b931cabd1ccf7d67dc95631bb7b836332865495160bfd855a20675ebf04cfacde7410b" },
                { "sk", "3381fd926243050e1a055e1313d7c8df88b21e4944ba3ff2141856dbf3bdf374656909a09ff316e75ff7e6de54e7d19f764d31b842039254c1632f74aac0044d" },
                { "sl", "9db844846368b9be931c8e7f0867fa71db4cd617dfcd0eb253b02d4e33a42f747540753dd89361c4aa45fe3025ef31b94b22b90ca4476efc88975284695f28c6" },
                { "son", "4a2917ed20b657efc55c40793a317832e03aade21141e17c27fedfd5b724382b1789e24b15f7eeeb341b814e2038b25f695d030de06840545d7ea4c423985261" },
                { "sq", "f98af022826e89a91890947e577c2e735676feeea4f7f39405d975b31e9d11ede36847b56f44d11a6d24ff9699f52ef607474dad1209ae698120e7ed074649fc" },
                { "sr", "bb07d1d083ae8a76bf7ec74f76a31763fce48bee049a2149832378a8f104894258083d18c561c0a4d11d720fd6222b5b75c0712d9441ad6a376328477203667c" },
                { "sv-SE", "08a7711a943c8d2fc7813ba36c05d80b8a6cfb9ba68dee964fc5c4e8b354d2204925ac40214a2561b31c85f48d46ba9f87e7f1fd6caabe10e1f4d9cb4421fc6d" },
                { "szl", "32c2f2be6e6ddffd3a75a7f9412d0f114ea62342ed846c59e5a5b2f9ad7ca4df6b845878da8807b4c324497024d01bbe15cb61c4f4388c3dd09cee614cf3020e" },
                { "ta", "5dff05cadd2206f35a420d05ad79448bf474d3f6c59f8e484ea725302e231dbfa0cbac0349acc046dc0ea24303d60a77c6010cb121c3c935766f4d10e0f2543b" },
                { "te", "247f91a2f5132f2c7f87a220903d15869200b7f8d3406a43594e914d381af53d0c389b952081a7a128c1794b86b7a8fb1f8c77ea2e246c017e4d44bf2e356a75" },
                { "tg", "5df138f15dd3e8b6772273d0b705c93e49ac338e8ea26e214edda97e6017208c96e8e45bfdc4018fb299e9ddf0af5778df4adc71417a72ded74975959a9ccca9" },
                { "th", "36382b902450c19c13d1c2bd57bfb5492fa0622f0e80c60a0fcbbbe3d609bb26dff67fd41021ad295ad7f700e4c2107cda013a401741792fe02e55c9d3514870" },
                { "tl", "ba3b2131cb1e5f9c51b2014e3285ce6ecdbf5b55eacfe26b6059809b33f723a7d5630b9bd1fc1f24c8841ade032c497ae26f2fc39d0961a7eb4eb0600c0d726f" },
                { "tr", "4be4e5338a84af9805289b99774289740be9f922d9ce0c4c1a3f9b518509c059a74accb738a49e07424b82a2ce2be4db730700d9fe2ec4e4141141107df8686b" },
                { "trs", "a4a62ba737ed7f614d14e3216b0e162fb21d36250a751e50d57dee9329354e78d506b338d6550f61c1083881d6761caa804de9e66419b8df8cf33620241dee63" },
                { "uk", "1eafc3cc941b1bd2b249eae9a5eae6ee458d490803c620a4a98af47b7300e82104271714ec5265a3967c7d6aeef2e753a7888b08fa2695cc0b4714d36292c21e" },
                { "ur", "8b3c2c429ac9435190312d1a8dea513d1bac27f757e5578ae914c6d5ebb13b7552381dd0c623734b8636cffb11a326f256a03ceee13d9f2116326acf98a335bf" },
                { "uz", "4e0f74886150f2128576bf99bd198d06844e37ea259b9b755efea038d400d9408d91efc4947228d731887c700a8ba554ff70e2790490826004f90032e7e16984" },
                { "vi", "412b3dd667f7c64ba1a1740a1580b503f7a58aadde1153da7a5bef7af8eb5333d4533cb387e8a6d7f90a93567b74192a7442ea5adb8250b51f69129e7fdeb565" },
                { "xh", "f623817d260d93ba2debce98501e6f79d770a82122592cce7a5c355de4a21598029223e5900923dd178ce1b25939fa09d3527d9d5f80f96552f2e0ab51fb3a7e" },
                { "zh-CN", "457030e201685e86388c7e9b997fa6c56c4f97a6643f7aaddc316e2077f298260ea26fd42306ed5aef7ae9e591e1485f16614d045114fadc094fd88593bd8e11" },
                { "zh-TW", "08877f19990d155a471b4147162a555fa179c3a4c1021b18b39dda91ff549e6706ca381abd371b7c8885508fdb2a3450cbee6b25ff927b89a7a4db3a8f25600a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "35ffa3b9801d34c99ca081984db136e233d2fbbc65bab5e1c53c9ded0a2406b3b8aeb3b01a4d515d96ba097ffc89e457be90e10e628b9ed4d7a514430c967c49" },
                { "af", "d74d6df6110399e4487fb0d36c943ad3addfcccef74a895af7c5221f917e03fa5f87206e818bf1439fe5c902e2436a8685b9599848abb904324d635089c8882f" },
                { "an", "ebb4709e012cdb958a92332a4151545436839052c09f1024c25a74b48d134e77122b5aecaa25147c0d59d4a0d305e8a1ca77653fb2f9a1b5dd8d980c3c7e04cf" },
                { "ar", "817145b722a25d926e4978326b07b13057c6dd21e74dfbb636551187e4d022911d9cc3b934dc7df3f26fa10de431d5dd446c16fa2c320f7e190a8d2034a94340" },
                { "ast", "1a360e5545b53c0dcf1cd9f75b66b63f6c5c7188cbb97974491ab9e0ffa8abfa40134e8af950725895739f94b5bb4b9275576944f123c15e9757167070c5cbaf" },
                { "az", "f1dffa6c8973872ab170b86ce67106ed25b8d780cac28f600088a1974bb51b27e22136784d56fb3a09eeb69906dd7858caa3c93c4833849e1b782e0ed35c4151" },
                { "be", "6696a067dc28714ebfc59caa1867ddbc16231c1dcc3db19afb4b253392dc63b6434af2dcc3fa499c618d338bd73a5adda337fc9c13d42fb772579b1495643fa8" },
                { "bg", "8d39ebe1285d26734a002a304528536a6912f60db13bf36f8d74bd882f3209e4d17962fccb402b077a90ee52537f6ebdf99de7d61f9f3322d8e72e6584a94707" },
                { "bn", "1740c210d218cd9932825553deed336a9859dc997c65692b6128d872e1633368aebe34e81b918b67f56d85a06a921ea1f9aaa2d64aa15682804efd999fe35d0b" },
                { "br", "f0c5847ada833b31b4555a471678526defbdfd8adc51fa4ed5c948c3c775c99129e28ef95c5a42e9f219a45981a662593fdfa15eb17c2d3c76edc449b0404cdc" },
                { "bs", "bdde57a0af97fc89a2e18058b0b23d786a8782e973bca1caa8bda7465577648953fdc804168504d6e8488e37dbf78dd7b9316e250bca2d89b3d8a0d78f317988" },
                { "ca", "80a5776953511057b87137c18fde98ca85f37820eaba1e158b05bd7319b6dc34d11610caa3c32c391ed46b9219e935f5748f38094f8f3516ec041a1c4448b96c" },
                { "cak", "06848ddfd81dff3dc60bd8d96439c084f83f1b697fcc561fabe8b6b934786269cef1fb78d1b956bb8f2a3ed27ce9452807d372c6ec1cf727b8d079d959c7ba8a" },
                { "cs", "409b91eefb8a8480a5e92b93f246f79b665da6866f5effd24ef22215289501175fe0edf825b09706ab3470120433371a62d15970ff24077d4bedf9ddbc6633e2" },
                { "cy", "62c9e2372f3e4d201d4fc0433b8d0edf7ba318eaff5900dc43d099ac6b127d190857a56a0eeca51b26b8176991440fff3c34abe1e10626a4ef6d4e8da134ed9d" },
                { "da", "a7e2b93927ad9d013dbf8c2a150d00c158abe50153b158f55cd22da6114bd94d0e91ab16bf5a00211cac59a8933d5617535581115b714a6deb7752cf94a84424" },
                { "de", "4ad1da7e8756775d4b8b1b8d25f07edc92a9f1396ee1d0449c414ee3ec63cfab5a634f6ae28b179a264ceb3e5047abe63598d1f4c58b7d527669c02e5f8ccdb9" },
                { "dsb", "91ec3033a76268f49f9b2c51a3731fbf424f7644c1feffaac3005b51c7c84821f3df09ffb53600cd7be61a2bd1859732d694622571d2377c478f7bc05f5a9202" },
                { "el", "9f4e218c91b961ddd4f33af06aff4ea606e225436de163193a7f286c0371f9a494a43bc83fe89e8c4a2a5d7a6d3038e6965f87afb882ce2f096b244117b9f881" },
                { "en-CA", "5ef2b5de4a602a21189d372d5e530c1f4ac0c3267c74e49a6d18a7a08baec361df27dc6e1349aa50bdb9e5685fb743a2722e400ab68a77cc337e22a4d7bfe29b" },
                { "en-GB", "35cd5f35eb4856025c1f94989e8ad6f26ea09cf1f5d5919a1cdb6e66ac835c0a652b9e41d5907a2ff8eba45914d8e782411c6524ae54487723535454d6f23ff4" },
                { "en-US", "9b1ce1636008cae308d8ffe75f02c641cc83034665a83910e3c51cc70d2fa61d6051f8087a262dfff138cb5af3dd5416ee272a16fd2f68781ff474a157a6555e" },
                { "eo", "6fa934fd8bc002adf75d1f9ff3244480acb5f78db12e0abb16e672ce1499a091856ac8fd96ff3829d1b4938664b5a2b9a22b614811658924225f3ddc565fc930" },
                { "es-AR", "2b85940ee8865c633902d1127f53ad4d99c9cf8eb04d0f894ad357625d0ac151edfc99a65bb05b02dd5695c02f042383464f3b4ac71d4adfd596d58ad7aecfd5" },
                { "es-CL", "9e5630e70d205f00ce4b39d3e31ccfe7d050c2092386fb5ea70a3ec9d3ce68c0d3866d7fa941ca63e690542719f7065a1fbcdf88efcc250771d600b98f1df446" },
                { "es-ES", "0d959eff4e6aff404c4ea5e8a585497e19640e8c8374934d03b3c048f6d615c9df5974e932cc279b04ffa12c9c1b24e1e7fd519f87e83febe879a615b3401421" },
                { "es-MX", "fb3b37eb9aa344873a96e6cca519b4814a98cf2d7158ff4aee6e9403e22ea56c0cc1021e3a20b6c5be6f89e36a6d4380c5468cab1f83a2736cfd6fe3dd39dcc2" },
                { "et", "565469c17b937c2e79ac471642b3486961a156fe6ff9076352cd07b716e097a1ac5e1c2839af52171019ddfd9be66ba6c55b28376c755fd8d7bb20126f7871d6" },
                { "eu", "3416d6d4f0ea9e475306deb1c9cda60e45a2f03bdac525b2ba414088634c6df92ebfcce0dbbd590574c7048b3a937461a9b4e197fee83c10ae0cc6ee93a06816" },
                { "fa", "64a2466a37ae923e50742ca31fdf135249155db54499591429a64b5abc7b54b56c1cdd19f4ad04ff6b0175e039d4d243f2c174b8e5027ecea2ef049d31d4ac9c" },
                { "ff", "af38aedb083e3d6f88376bdec09c2ecf289a9260ac8c5d6629866582a4ccdd518ac253b1fa83106b62e5f397c54f474b29400706a75d0693b199c4af6c534d45" },
                { "fi", "77dda483bb035892fa8a48d222f00d8fcc8ea2baf0f6119b7809bbe5850e42b53ebad3f235c14e36da64c10f3451e32fe0bce32e40ca30c903dc386d8b6c45bd" },
                { "fr", "36b73e050904cdb47e7d80eed782f2114147e3c9efcba2541ad562216fe463bb32c70ee640aba4b2de3a6ea72b3546aad50420e39eabbbca10ac52170395b301" },
                { "fur", "e65797f5083d2e065c7914908772a5eac5d6be24403717c07223daf29a8b6058b6dafaf4b0099d30e675d3f90aa2a448efb4ef7357c7b23eaf16ed3600caba07" },
                { "fy-NL", "bf9c3ebc11125a2034bcda8f03aab28eb9227789f5f7ac817d5996f233a2fa926e50243aff409be61733ebf0d1826a6d845b7a80ada7c7563a87381767a612a1" },
                { "ga-IE", "22e02b5cf7363f84d8f6a7bb54a10c7289d2872a6d5e49a61ad98e496b1166ac24137f641fb4c339be4b2c6babad897961e181d0b626a4ca2998d5c63ba429e0" },
                { "gd", "9df8d78f6cd79f8d727025e5773f37445b37f2302be10509e9b1fed10ff71cec6bea9f57e4d41d5b27ca9f572174c090498535a5093377cee110082cbbc2940e" },
                { "gl", "e32a9e184b8b9ae96b8e3d33b7bae759e462d347d4404807d14ef8c8070e2f009688dc24700b57a2fda11efd754aba33f4ffd8aa0b93bf61f1f938276e821b8d" },
                { "gn", "4488a979b2f98e789096f5b038a76b50e22987c44d9eafdfc3cb49e93aff2515c90ebb9e3130af6b72f9f30d9394de2da13385ed18b9d3fbfc07c2d065d41897" },
                { "gu-IN", "b7f23da56adf00589b874e90eaf5260fa94f300fe2293629f68056606527531784705cc90b5e922fbcd2ae9addd6692c9e41cd21236098937d0e8c73240312a6" },
                { "he", "2c05937c3195bc2e25494170be820cd3e6a36f7b7c419cec4df7987180255ddf9ab52c78bb3a8817e244361a6773cadeba2245611470b43749c816fa30a3d26f" },
                { "hi-IN", "cba5d216892ba442ee35f752ae7a9ef6e1389493588a7c20e0e9fbfa8fb630f8869a53b1083c7289db4581a7f4018d849e6594cb1887a6e7e5889a5db73e21a6" },
                { "hr", "dbd6a82ad14a0a66de624eb17fd6becc78bb0cb881a1dba7245c46a875f4a247eaecb4510d0e9697c10bd348873bb55276feb1b792f013b9685ed4fa2a49868c" },
                { "hsb", "f3252ffc1a2aebb2c81b485e7dc6617e5f0476f11cbaefeda35a7890cc9db427f165935678a3b2b287a30f7d2f8d34ddc1318df30ca29df6c43c816537717739" },
                { "hu", "505dcb0eb2f17412077423ef6cd8ba5d74579d3191b6e66f0a26dde02fcf9bca2d49ec415f381d9e9f426a7c66667e3c8ef58ed824eb2a9a1c6e670d6f7bcfac" },
                { "hy-AM", "f592d4160a940175f3c8f654abeb0095630bc03e6fc423ac28f56b6681ed4e619919f6aaf4a96280446828023ef194a684362a6a82ec23d5990eb4ba9658dfa5" },
                { "ia", "bff7b58571e88be0733d9965967c8c9479eed23aca0cb803592d799458fe7312ca16e2af8ef9ff6edffa35e8340d5362f68916a10711a15d2e3e7a62d4eac8dc" },
                { "id", "db0bad2382f00f35f5227debde0d2a44843d6275c8887a827a809c1623f26bb1e0352da740c7ce2a1a06059302824ecd2a0712ecdf82d9d8533889c476b6da90" },
                { "is", "53c104ca3fe2c08d9ee8d13cd7de3bb86cd639d9d7ae49da4b93427e8aac7a0b5babe72d04f3288896a7e3149c1a8cc645c558bdb6267cf29900cb8dadc8b04b" },
                { "it", "44aa0eaf66fbcd1ff96655f4e853c5dff7d1d64567bf12ae40339c924d0b8477ae3b41b0306bf9b72a13142a31c7d1cda563b6b16abbee0520cbf09e664be46d" },
                { "ja", "8676617a6abc55cbd1fc5280f4e40f5f4428880843f95f7010826bf3281071778a0bc1080807f563631ff53c894e54885036b6534f4a43050010e888e56ec2c8" },
                { "ka", "aa155626fea147d3e05d60758f12b25a3f6cd3e0c745b12414213e5f01af43ede395f85f4c20a95da5a93669497fd36e181f87475491e7899da37cda7f897fef" },
                { "kab", "d36c9e83d5715c8c710a72db327598d4e7ae78cd0f571577e8c0bfd389cbb639400a84f9bf58b7008cd6525f4506f40406f507e76eaa0fe6a4209968f57461c1" },
                { "kk", "020bca59a27ae400419bb3cd5eb9e828fb60272625d4c51268b303577155f123bf27786ba9d0a9bd46327c8703b978e21c0b36fef2e67697125f27dd18add04f" },
                { "km", "926595eb99c4f7c4489f1af3b4b7d696f5282e46c6bbc2672b75f6007f3335323d1aa45e7ab71290619f15cd8f203b05433e966bd0894acf4c496eb397064715" },
                { "kn", "8039ddc0855b63a1c5c790fc51143a94f6df002d010d2a3ea0228e60d2af976108cfa1b7b88a0d475b70baf174c39b340ea44de1ce9fa683c66eca80ce2fbdcc" },
                { "ko", "6108bd477c936fed235a37cfd536f95fe3d33ef0883913b869ff6e1bfd229280d68b85c5981ab58efc32c51b7198129415b15949ead4ade52afba51baae69fa5" },
                { "lij", "885866342b3db27a6abb3a0f71d7f3d32a75dd077ffdc71f4aac71d6fa99702cfe07142f60d9615ba1edd51c8c7c1779afeaf3a0d9c0b15712fac423841ca263" },
                { "lt", "b483cfd41252c348b451bcb7e0fcca857b92b910b302793f9db59deb704ba935f165d1b31970304c6ba9b79e8d9d22a810c8d6391e174eb91488284994f5dfad" },
                { "lv", "b1cdf41777f66ebc818f9357e3cd91b9b9e2d67eae0e580e6e7c1c133033130143fdf8f3350683fe13b4685dea3cca0caeebb61cf1cc3a59d3e717e9d261f62a" },
                { "mk", "ba1d44a31138187097a4d4c2a76c98ebdb43f39efb1b7ec4430afe9a013c1145a0024bfe700e531e933c037ce804b690b78361235de3832934d8187f6715359b" },
                { "mr", "150d039b8ff2682bb42b2a5d8d51e98c44a242276f5d8150714c8670d7dacf1b0b96655402de0fa0608e7dbed46154a2bea9803e3e0675fab3ee019d09f70b22" },
                { "ms", "598ddec9cff846829fa849f29490a73eba11fefd30113a361f0102aed004a34fce7f23616bfd52b8d84ded90f785056ec6bffb878951eefb50da0354376cbf79" },
                { "my", "711b49b2b218c4627d339f92004c4a28c799885aed9ed128421ba9bd1e0d768260d07ab7ab2a4f87848e83f40123418377a2e897532752792fbe4faa4abd1cfd" },
                { "nb-NO", "f164f66c8f8580d6f6799611170908eef9c721c6d9cedb38fa7ae2c52eeb38820ab51fa56d7d136409cf26b99cd65fd87e79057cee8d514493e10ef3b4c3f323" },
                { "ne-NP", "9a46896779678447cbb19ee1380fae769183d7ca04756a9555af9d03f4edfae47d4ba3a0a76c99a24917f16be29404c153c9bb7ebc741bd180e3c37aee8f5493" },
                { "nl", "e68124c649dcd629e2e339ffe21664c8711c02c035754c23b7158a107bb65376d30c1a50107b9c7612d26f2eb3822e1fcc2f8e657d0d4f2ea986fdf2bc3ccdb1" },
                { "nn-NO", "705670de9945c552d87ed7a3917f0764800f979807b919f8ea771a8b2bdf19e66b0c2e6e2b9d0a5521a7b8402c9314b1f2d71f746df35ef521a835efcedcaa36" },
                { "oc", "30966ae7932e362bc2c0e1932dae53c7722580c57cfc2e4e84162467d3f49c7bedaa2e75468ade73d260674407b8a8f1cbc141f11b2af64118d06019d70218fc" },
                { "pa-IN", "b4624a3c70cfc6f340a59461d3deeab4e46e4ea567adfa12796e6ecf4670ba09aad8800bd98b3d161db5edd8d9a9d34e860d35b852ab55c011102b2b107ffb27" },
                { "pl", "d6054176546842d6d55e6f04483306b00ed8a4c705ec1adf4ca6feb020b16076cad36356b2976836623c56bdf1474b5ce969d9d050782cd1c0358cf46e4af4ed" },
                { "pt-BR", "8be008f2bfb6bca85451925378b990d853c5c1d2c104a4119323c80ac734b0b97dbafe04b68214fe698e7ba79cb255f2348209fadeca0b58ad47fd85b56d8e79" },
                { "pt-PT", "23bb9c60ff6991b4afc85e344ef7406777d8569046eb154a4a8547a8aed4bafd3a7e0cd6facfedb4a18aeb57feb7008877f04bb34cac8b60a4bc6e33a28f3f4b" },
                { "rm", "508f47c3f6617d50aad1d3bf2aaad3ab61ac7a09d748e0cbe2fbd4be84eb5e1b5284b7cf4b08fad0ba1afc6a35859251bf0ee4a9da42ecc187c3b5b4a2d2d4e7" },
                { "ro", "561269c05c7d432c09a824414eff7b8c2bad857aefa112393e26e885716db821f57ebc03b8e6c5de13bddded79baa0414d0a4e67dc69c98fc07b6cf672ef58a4" },
                { "ru", "3810e8bb4561b0d76a05be2bb5455e3ceb79dc0e6d10d2510567b6372031c7e33686bbea8478cc0881135d1ba5c1ae5a50b40f9c67d2afb887192de83f5c670b" },
                { "sc", "b6402984cd8c7c1defe05459ca8a1530cd335545a62598bd7edd02b5a186eac6ac58b5f2e39e3ffafb70afd37df468300dd06c090c20c0f7b46faf8d35ad7176" },
                { "sco", "6c0f609ff3ab8ee41fd0d26b421d0b90efb95f12b4d31d0681041c1ac5c8453d1c06f4758b37f0725ed7c5ac053ef9072be9b516c6e5b1789542ed157d22c70a" },
                { "si", "771412019595770d603167173cb8c55d0b474b89140812aeacbea7c2167224ac2ecbb99438b0b6a549e581ca9e1a046a8f907ac173962172c29218e0b0ca8125" },
                { "sk", "3c4ba15f76e06b62ca54d99636184b7af2ede452653a83d50c28ce68d855c74ee5603258ead1417747faf29f2dd2fdbc8e33c360226ada4b66d5912a8c994327" },
                { "sl", "a58cce122e933924f468cf74249decf4ffaaa3fdaeabc85d6b5b1005a8305b8035b250f29b5b034cc902e2b80aa9993324e85d64eaf85dfac682df4d2c4c7481" },
                { "son", "bc96c7704f95474725824d1422499c08d6639b6e10e08074963903baabd327cdbf1df592ac0917398cbf91788bb7d713ecf1a5596639ad6cb4adeaa357c240ba" },
                { "sq", "11af0af12e97a0134bc645a1309029e0e8723ae9295c39f44f6c9e854621b8d1d7f9ea7a5c04c0365edaf58133689995913ee33afb2bbe990b173ba26a61b5cc" },
                { "sr", "ecf90cecbb319c3a4ac4d82c431675d62142dc774b9289ed041141d506854d260ce7bccfa6db07d22d50a09ef2829fbbb27cabda574e475419314fc2364bed94" },
                { "sv-SE", "952805332c841841ca18a7321d747748ea8b66d43683fd4042ace4cdd02034fca4f2d47910e6c80bb29200a8dd66f3628418f70f96192000ede6e31ca849382a" },
                { "szl", "a06d026dbc4f9653b987907f03194cae170979a6bf0703626430343710a994a7b1a02018706efc1c654a34fb943f0293fe939218999732e7ab640ba3034c70e7" },
                { "ta", "8275795ed1e98ce19a598c19b0edadcc52c4debc3e9ad2c14e3be614797b22ed8d425fb6ca6de7ac95f86f9d0b87d9f79906aabcf90d93a2b1fa126a6af0da2d" },
                { "te", "64e88e7201e4e9caaa5b986794e5df62266bb106e0fd8ce76628b48fa4cd8242e567cff01f20434bb9ff910b4cb1cda2b996d29086cfb6d8f2a602880b8ea057" },
                { "tg", "f592868fda68af26281a9e5714af263fa06c7cd373472e11876806c11ae520840bd86f29d548bb82f67490e5db451fac63dea58e2486dcc9a913970703c4b5b3" },
                { "th", "63c3fb83c6552e769484bac447fc0bf288dc67e6db4dad8ea8912cdb664b8cdfa987109adc47d77f7842aac4c999948395a34958013de27a6da5b590c8606de2" },
                { "tl", "ce1f7f9cd05dd214a9ffba82019433a777672e84008027a187f10280535c02435bd01daa6884daab91198802289e15a3ae0a6f980bed4768daa8c2c780305dab" },
                { "tr", "614da294b045dfbac57c21ab799aaa1b2d45e3864311b9a30bce5349ae1e8e45b98d2e778d30bb014a0d6aa8f97288f40f955fddb9e57b728215d427f91464bb" },
                { "trs", "a40bde8bfae20ec059b13c9a59591a46229f9b8345d17263b4ee4922b74f6afc69c7c6e1043c9c2d602a937661b8ad178d66c6225d37401893830071d55e3348" },
                { "uk", "07898cd96f3495fc39cd0da1f70af3ce00de627e3ea7d2504fa85d6aa470c03b2e2f8f765a1a84ef1c086ee4ca9da2f1562670f888e2ea92e33fd6f697263366" },
                { "ur", "a3438fb3dc382b8974e9a87ac37341ef3aff53df6d20960107ae6ebd854f2f542704969ff223a5fc7c1d68cdc7671d4567fc907ec1044f1b3aa115045be81d32" },
                { "uz", "30405928c1617476439b304af86444cb44dddf2ef6eba6ce34777a352388eb6aafcc2e6e12d080c4dd1c71ae838f920530488f22657f8fc374d7ed0d8aa948c8" },
                { "vi", "fca6f30442452940530c217bb1a6b18877fc7159291a6823a50afd9ed3ced6d3db83bb590f660abaf246b8a9eeabbacee8dfb9bb4340ab7469efa3b659896ed2" },
                { "xh", "2a29311ab9b3eb2a49d647719ff01d4dbf6dbd88722ccd828bfd581b4483577d0f56bd037eb2e4a5e543ef71bd0be73a3ae9d34b32012ca18fcba6003dcd8372" },
                { "zh-CN", "b6c7da7cfbe997674233e84ea3993d08d2d462f66f820ca48771fcb3968c9e18b5e6a6d82bbc311ac23429288d26561443677188f0e91b97049a4a94019d2854" },
                { "zh-TW", "40077d944a16452d9d2ccd06dd5cab1a89714f4fc7a3ff3f214d1af18b86a48f4098ed671e7c5bea3df363c996a973eec6b82e4cc95f45b45ae4c83e6ec7cd23" }
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

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
