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
        private const string currentVersion = "117.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "c9241ee971f960efd9349eef0189f912a6ead078efad307cd71605690f9ed515c861f95f1549ff65fdb594d4f8dec82247b335c8d46451efde75472744c06d6d" },
                { "af", "572428d4dca41ffd7d4350d03412c3c8dce820f6e591aaa4168339cbec436d915cbb8dc886659e2af55ad47b0a3fea9132fbf6ab64d4af0c4df8296ba98a540f" },
                { "an", "a6350b8c2d74aded187e6a763c40d8a6485f6649f39ba6ca9e03a10721342d29ec1259cb8d90b00d5224ad8b08c204c4564b181706608dc2034db24d8badd1b4" },
                { "ar", "4148d6302b9572e32e8ab012e7b97307740b0b052454636cfa3eda1fc82b850fecae1b3547798294a7888ef3021ed461d6a3bc80b217aabb12aac597bddd7683" },
                { "ast", "e22a7bed5ce5115ec8a79f5fa71e78cb6b91c6d7ef801734cc86db825a73086e5d05f9491c801c5c6295ccdf4f7e6a599404a5c7d03c4aba4bd776780874434c" },
                { "az", "b061822eecbbd9f3731bcd7854af268dbd4ee84e104e29bfe1e67fdbf9290a0d5777aa218623d536bfc6639caccca05b111afe0b3722f0838fc4697a1f638282" },
                { "be", "a690157b377e71685ee0a475c87dc619da43e28b7484a2e92b54146ee2fb345553e14473e489b425ca7b1a772fa91550fae481ff1424e75ea25441971ad3cc15" },
                { "bg", "56fed4efa2fcd5fd0511face93cf0e8c76fbe4ed391abfa31256902b5aaf0598d0d2f9b90f3f6e0c13bc67caaf601cd6d338b30476ef2cb22c0ce297a7e8c5a9" },
                { "bn", "ca3bcdcf5aeabfd17d6d4c6a837792478aa860383332ce1b484dbc950577a156066322bf73ef570e1db2ab68e0d5301ce83c576abf6c1b345563a0b97b56dd70" },
                { "br", "1aef8852775c0f59b32515acfb465bebfd58838a470f7c0735de1c0fe4368e350388355e3e864612ee7e40c005de898734e1d92211bd0f074adbf8a284077f83" },
                { "bs", "733018f87cda2602ddf394fdae49b5fd7e066d9fee2ceae6e88cfcf0c34de4eb9002f82630bfda7dee6640525008e43ebb181cf3dd9974876fe872dfd702138e" },
                { "ca", "175cdc4ad6edf6090475be908fe0d8a0bfb9e8efdd773c43f6b87bc35217feb2b6bbb018989d8e023be36e23719b572ce0585db989484a874b0cf105683a191f" },
                { "cak", "da8d6e4006059041715ae27bc2d80bce9bee4fcf8879bc56f86afac0063eb2e987cda5036a12d12c71514358e1ec7cd2dd10e4c191025ba6b0ba868b0a9a3d67" },
                { "cs", "faa5e6ff72461cdcdc768d9579019ddfcf2e96bb02018a46696699972e4185b244b6df063b1196b91e2d62042ac32eaf2b6a75a2f863dfcbb93da3504e24e613" },
                { "cy", "eed3267c08c0aee64e2c21f94bff7a43fe0378ac5661105043f98fe5cdf997ce9fe47679264d7ed35478499493b5b7e5716d9877f55f9b8b999272e8f843cc25" },
                { "da", "9ef92be460ac97ce1557f08ac96438739ceeb2c770ee5ffa58b00e93dc91b79240957c54f33217eba26f3a1b3d6276a31f7c690295fa09cb17a409e19c5db08b" },
                { "de", "a07fdd5b60fca53a62c0c1b32f01a857da45e07e5a6d028573fbbeae19bb0e666a637d864f25b9d709ee22f0bc115778b68b2bb29e76381da122bed5cbedc892" },
                { "dsb", "fd24b4961d062da3e1f9ec21774687ac72d910275be03a85257b049f37f266256dd720ed571939815d34e163a29428cdf87cef5556bdf0ec46cb36b47a1a64f2" },
                { "el", "0b6e2d08ec0fd290dc86f6f19df0b46d109881fab267e5bef11261d28a221af07592b3a5d972e2056f499d8dbf26889ac9db66ac4a6a589d6ef9c988573be93c" },
                { "en-CA", "869ba7a8c1386b369d1bd205c2442300265cfc5f9d74192a6d8010f898a0bbe1e7a62af66f6067aa1dff8e58e3b5938271beb5e4f4b193607328364b1ebf4c7c" },
                { "en-GB", "dad2ec0b5f6929ce528e4409ca5d2e3d69fcd6d6b8fb68cf0869d5aa107c0d7bc196e8c2f97ec851032bc528c992571b73cf3004c1562dd2a1360c6f6de2b8ed" },
                { "en-US", "fcf01e740e2f00a156ebce59f309bc8afdb9852b23df9a1edf09dc64fd19a684c85b4a3a346b6759e93922b58efca801220dc79e6b953ed64f43287efcff68b4" },
                { "eo", "85073e9adf11100b5b9cfea224c481fa097674042464c3d0a96e79fb0f9bdbb5dea440295b2d28ee3edde7d42e31e85a672476bc2e6ecccf680b43ded7c6ae1a" },
                { "es-AR", "023c957035e509f4aee09c65ed5a161276add74c5e3116f46bfbba196758210e18f90cc0433ad83094cc19459297fa10feef8d217f1c46ec3d34626cf523aeec" },
                { "es-CL", "77e53193dae661b7f774d7dafc554feb6a2304c2ccb8f37bbd1aa478d51dedb52e0075d45bef45dbe5fd84cd887b27cfa8935bcd4097d9e3599f1ebb03f0d866" },
                { "es-ES", "220827db0d37bf8d45f71aaed88a92a73411195bff550e2a134f077c58e2f58b0beb7dfb4a9494078619344bb2413ab3753aae56bc15265e1e6000160bfc78c4" },
                { "es-MX", "bd6b2dfe149697152541c18e528e6a408acb48499379260e3217628fab6c6f011d57335f67d6ea217a9968ceab3bcf2a47b2fb58e4cb7026acdfbba539d7ffbf" },
                { "et", "3d58c5d1a763cfcd49e885624c0553a47989f457516aa69b4857cf215dc652a94e86f3e498e3a2146a133d47b28afd6ace9920de203ae83a8871d163b3ad2f34" },
                { "eu", "6476e397d35bc2df96f30d2189b1639a0a54b8283133df0b6f69f7e0a6a2d9e774ec807ac1fbabbd88d336150c5d9d5d629760c44a4ae4e0e6866d5324ecb16d" },
                { "fa", "0d6e332ed4437da1baa40bf974d9675ea35b77b829dcc61b439683bac6af589222691027b1cb4e81871b44c13443acdb7d773a7245396bb484067df656ff1247" },
                { "ff", "dee1228b58c1be629881918884f1602e868ff7bd2160c7104d0078c6d307c869c3fb75f4dc3c59d490e7269a756f411839e1d6400dcbd66d078f0b7c58aba36b" },
                { "fi", "4b8274169909c70e616fe59cc7a6b597c9c4ef997a023e2f6256c10a76f03a7f09ec86d6e255a83a29a85bb17228869630a6f18e722f3949d9116a70b85e7ed3" },
                { "fr", "06db081f87b777a2c5bf1e7d73626c431bf07f39768533530c843674ac8d577f27b6e1806f6c89a75d9d98fb76c3c2c7903d80663366fdb92815bacc22ac2aba" },
                { "fur", "8ea18504b2ce7595b96af58d6978ec15543411446b47ab15386b6a693b7dea2cdcf5e2d949e4e5ecdad6cb16ce63df4ce8f9ba99ecbdf5ab07c0a1e5658f47de" },
                { "fy-NL", "8c319bb8e5825cb562c2f1b17685e84d0d3b2eb3c4f828966e8b0bcf00577451ee166a811b0e5d7c383e4bc4f573a604bf9416f4c926740bb397bcfcf1997986" },
                { "ga-IE", "d97c7f4cd43a8bea2e4f65aa68e88183116284b31842c014e7428f0bc8db9851662dea5ab1fb0164e9f2c3cb27ab72bf8df5dbaad4976ee690252872f6b1fe29" },
                { "gd", "8a992ee7d8daa4f5077e87ecf3841e25fbe37a24877700636f65489a59e59790370e74f502264409e58efc5a93780d609cb3230cb1362099af6aee97d3096d5c" },
                { "gl", "0e3b951e1fc91bb6b8131d4e7d44e1a99a2333bb7c24c643fb7ae225dc7fbaaec516c0d698466bd68852d2f28417b8ce2cbc3c63c64b881371e64b1215d93768" },
                { "gn", "1d9ffacfb0e2a3d58f881b9a58a09b8345efd842eac9880c5339873097953f8136646d82dea844a84921b0e05a7e6c77bb57d22c4eae906abdce9d53345bdb47" },
                { "gu-IN", "67979d3dab332e6a32481bdc57e508ab1f65fd1d3e6e1bc797e978537b2b3a48ebb9a4ec0ff42a917a94362a4945ff1537bbfcf370b0a27da70ea1078dfb98a9" },
                { "he", "ef0bd99c5458857a7c69fc7285c296c7dd2ed1c7a8b7354a6da8b20f77c79af0bc23c35f00514a724acb3a6aa48bcf28080be3066f0b24f2264717ff42220559" },
                { "hi-IN", "a80d77c6b2450dd9d0f547759e336a19014ec86715454ad7d6359d3eebb99032c63ba93708e060c76f7dcf2490dc72cc7a4a6f304d296e0d730f1b5f323f1f84" },
                { "hr", "4eb974ba299faa2bd7aa4f9649e09deedf2c42ae0e1f4e5a4b05b362a234f7ffb7db6a015e51b74723fff250e3f0d22ac92d52ca69bb5ec4003d50b02c69316f" },
                { "hsb", "080d1ae204c7dadb5183707363aadbc5342dbceb6401a0a611a033522fc030b5f9054df3f5592fc431a93e97c317c96aa8c0f072ef493ac92abf684237a56478" },
                { "hu", "d7df5e68ce9bf2464471d06ede08b5df4478e4e5dbcd1b047dcb93cef8785c01cfde5c4c0cd46b939fb5f6e1079e67f0aac9ca8debd3e61da494f5f2e9321331" },
                { "hy-AM", "0549eda54f4d27fa852d769783ca0bf54a9c2e7cbca822dd8f01ae22099d4bbd873f2dd4c432e7d96002962b228e2cc04bfd52e0f0b96f7a61d470b09767a2d6" },
                { "ia", "6fb3de3582833ee7756634de89b961c2f2291fb57ce90ed1aa25563383812a906c52b8f36a40d57c9ca3e0cb6500ad52df1327141712840de031bd35fe3e2c6c" },
                { "id", "a7905c21b38c487deef0212058e7f0ecec66c29d2cb27d8e6a7cece51e95c4dfd1da0dca01c88097262503155bf003d29096115a62bdb7b420638a5308749b96" },
                { "is", "982e93c526f552992251eb60c49700e14a8e97d6b075c3ccc8c17351e2c1e9721c077d44af554afdcaf64be4a3dd12f827727b771b4984886a2a0ea7c219ec54" },
                { "it", "5e3f021fefda9483d76e405751c5ce0fc94328aca3700a709ed623d9803091af1741678b749692e1a24769413754fa3bfcd0e37ae2c1a5d221222c6fe055d194" },
                { "ja", "c940281f5905dabc657c6d9508a54db18e6c411c9f011a5df3a5c810ea9e8ad6b67f5ed00fb41bcdb417ff5026a037c60c3cc3623595414815f2f6c09a1b43db" },
                { "ka", "c380032213cb52a6767466d745c0b8f7e19a1c19e18e1bc1c30099580ab5df5ec92ffd6905de79fc9d02c7077ad3a3e950bd8864800d672a4bcd8a036925c4f0" },
                { "kab", "6e4273f97425b5800675047281b6639f0bac794e56ecf4abaadc9f804d996b1b12ebccfb67663e868189a43650cb44f0559e1fed9fe576c4573c2b15972a3625" },
                { "kk", "ca863d50f6b7c20760032a28acecef017d2cff5d2cc3f63a5e5a4adb0e8434d631957fbdad166618524053c9a240e8a397c07b1e31a4b83145ebd301d5bfd3f2" },
                { "km", "3c4808c7a895eba5dbd227c84e868f2d690c897b0476469de01978f0bfbdd2da52546eda78795142850be24614e90e7a672b66022f02b0a43874a065f8a6494d" },
                { "kn", "e57cf7830309b97675ef6a2664ece7c68d085fc627aad804cb57d0bce44dd77d551614640f3a3ed19dea176449097f4088512598207542ab5e3cd7437d4eaf77" },
                { "ko", "408082d789604e690019d109b956ebbabf323e8e2de650bd9bef72c4ebde216dc78435c86c8a31d6540da4f553c84f33a2f7422a104709b02b0e6b7a4a50a7ca" },
                { "lij", "f5960c20bc7c09f093226c5fcb30dc073ffc8155b37e852fd56ac1dac1b687332a75ddcbd3923b64eaea1615c096fc3bbf1770fb815853e18e1d12b76af7b069" },
                { "lt", "8c5957eaf1462a31e34991aeb0d572c90a12bd915332013d2a5f23587a3903d25e079299c396372fc4acc345af34598f4f5757ca117b90cfea3d09d39903382a" },
                { "lv", "d8b16352f6aa08867a5873760cfdd3c2af82fe2d678e6a0b99f974cb858e78ebbdb79f14745bb0efff8fd6d75cb03905d946047e2161751b5c051e2c3094a7c6" },
                { "mk", "ff91708dc8ad9ce0679fc0079e53a12554f3c5d836d534152f51f35c34949127a4bc808796d999980015ba007d1261b6e161f6046adb29b917cc38321ff1f43e" },
                { "mr", "0a40a461c58c430ffbc6e49b904b11786145faf7e4ec14511549d7ac8fe9f1e57eaf83f98a081f06da274498ec45a4f101b56a2a574436727043cd8aecf930b3" },
                { "ms", "b33d0bab2356b6b64fe35cdc3032d029379c906cdfd6eff2be62b391a34c81788ae0707388f9bcfb83d4ecc7255ada926adc56d6612ec794c0bc7a3c29febb8c" },
                { "my", "d922418d62ef45a30df83aa7b0c487060d86a85886c27bde020b9047d56442191d83e5fdee49c5551ba0f6fe393c7b1b3b41de8afbe8601068eccfab7ee4459f" },
                { "nb-NO", "0d2412968d40dfe3fd8dd4d3015b48ea5bbb72dbb71ab274405bf380dd0331624b03af94d1bf2545eb2b4ec941283236390ccceaf5c4f0780a6ba5ecc3220f21" },
                { "ne-NP", "e8ef9c5c5bfb723d54c1a2251a6c7e7b68f16bc9d149943f399eff96ae5fcf8153e9204a61634f60e33fba77639f835d0b197fec1e6cec0da095719ba3b8b148" },
                { "nl", "a69b67db0853b49abfaf2bc977d697470c3b3873416756566ae052efc517bfb8ffb38cf9de144f4dd085a9f70072b583269541464841c78a281dbbb6712940ab" },
                { "nn-NO", "80e5ec3283183cda2588b55fa38c25e13f60d8c628770b470eb3e344e2da4d2cbf530ee893afd7be53f143d02ffee95edf0639a4a421ac88987f2969e7f17f8e" },
                { "oc", "b37b8b86367c4acc864187f104e25dc38196be848baa2e785278be403f922213e61a955c0a8066fb082eac53f71b3cbac45fb42032033139fc8ebb48c730a45b" },
                { "pa-IN", "ebc3d5d32b92e02f1ed487d902b8e81d8a6ef5288fdced7e63ba0ae597eafbcf7dcb2252de60352c1769b0f4b69d1652be06f9eb631771a2efb9a945b8065555" },
                { "pl", "5a834ff388eccbf722e0d1c1ac0d5c619a1413858220b618aca883de27276c054877c0e4da6ef7f57007adfd9d55130d01542270178ea2d992510c4961bcd625" },
                { "pt-BR", "127a7e9369df97e880a7312475314148d75c004ee5728481067410ee53cc0d9aa66a9b53d3a091c89901ef8c9d46b8c77205291f8bc553e1a8ac60c71df328cf" },
                { "pt-PT", "cab881b02654edfc3c7d1264589a0b68e31bf900a63538968b05f818726242d3781fa972635e2b639adc0e6c6625bfdf38697acd5e640b8ba0ca5046fab947ca" },
                { "rm", "239bf79654a698a14a3c64f46ca911b4080d7e70335ce239d8747aa890ae2799b5a65d8d2cc11f9a9096e98cd712eb300bc6960981c84eff2f973be1e16c8d19" },
                { "ro", "0878fb6b6f05cb3da4ba1e4d26abb03090e429dbfa664a885d804a585c704b3bdd818b5bd4c3cce646ad3885670f7a44227258424832551cf66d3a4df72ccbc0" },
                { "ru", "3dfbdbb8d46b6f4a371235a8108db4c8ca52f2bc998ca068a34bcc1b5fbb704c7ac6069e2eece19584298a0bd267b37e05c683c3cc05a41c4c5e3507811b434b" },
                { "sc", "7f3184f9f8f2849012f71d95d0d638c5a1cafdfe7e20ff828a4e7755f6230f605d06609b6fe8e23ef8ac2d6b7cf69174f4d4154b37eaba7c49d78fee0d33fdc6" },
                { "sco", "eef878c0ca4fa903dfe7ac788a512b1e4d1b3c65dc0496a7e9b106ecdc40382d9e39cb9672d55adc0a4e1f43916c58017f3f8591e04813b5a3dec5447fa24191" },
                { "si", "404d67f275c7112e67040d937f7e892b407e981ea35565c62c35936390ecc25e4788d94f79606259052222d570b60a3963510c13eb7ac5222dee4eea8d171280" },
                { "sk", "ebcdb67b1bbcc837017cb80256989a7a834c4df22da4c294dde46abd751acf00587e48ea25968634b14b3cf14d2f6827dc69f7f172a0bd338d3a942509a40a63" },
                { "sl", "2507d9f54a9afc13ec55b923bf37339d341a165d08b112154d25a9e95d196bf71af6f4dba619ea6e282003b20c78e64531eef55e64d43b2d84cae8c8a0fb8cf6" },
                { "son", "2742fdce65788256944a471b9b6e8fc79cb0743b6af2b7fb7b64ed1f4fbadc67d01be9009a6adb217f18da8d1c2e7ea213e137a1a6894f4ebfe70de40e092136" },
                { "sq", "fc3ec1849e550a1f743dbca5e7bcd58510e4447c5a1ba2abeb84248286862c4a9172bae29e045532d27eda7163046fbd8794da45565bbd736b19a7ea1e280743" },
                { "sr", "5a988054f7352976f0519b56e67d957c3bcb0b60d945fb6d58ce673cbf22623a593380f01dd5b4fed88c1469aa46dd89aff5771b08faa9286d002c8a3fe541e7" },
                { "sv-SE", "081eedccbc840c6abb0973dc7a555a74add4f771ce4e725d5941d29f643ccf47cffebcffc2c7fce5a1122d8a8caa1206c8d9cd951e8a0d3e23980eb3efdf470d" },
                { "szl", "4a195ee2f8ebf2ada2d111975ea9fefd40ffd5d7164bbbb6a966246c3b0774984b688f02c6743e464a1774336024b7ab19fd7f299df8aa7834d6c34318d04246" },
                { "ta", "d0f3ef2ffd37a299af31cb54a6feb8e74468b2e490bb1279338f4bef66f6f13227793b4495b0624055f3ff414319d69a6c8eb7f8e8de78aeec1547def5f84acd" },
                { "te", "a0a76ff7215da4d3469fa44e7d49746f582e6b327ac99a22b649044e99cf110917c50c90b441b5214222a63a2fd7978d6932316cd639f7cfd0686ca2d849b472" },
                { "tg", "d4c9fbdacbb2d9bbf3d4382fad37ba9a0a7171d8986ccbce06f5e0d7992f0cb13faa266c276aae7617eace18c4b9779832e02f7675e3682264de6efa77c051d2" },
                { "th", "f3bf09142def6ade28590724769c28eae006748548f139e13225f5c3bbcad53ecc76d9d38522cc2039e75f372bedf11c606db4fec04060cba8e7352ae5ef042d" },
                { "tl", "1831ef83ca2a840dee4a47e90094e6c26f2ca96030c992925d42653941227521a1f5777cfca5bae7d893c9770782e72c971672cfa3aa06c3e05c1b1827632b23" },
                { "tr", "f68ae7379c9e4cce5a38079540afd52c1b53607c940c2693014b402b6678b34c19678ef36b596658c74436067b5c3df8793b774ca13b01fffc0f1c4e97f72d2e" },
                { "trs", "b92d495bf849f5de1f88f764bc5671819efe40594fa7b720ee20ead156c40d5f23eb35e566419cef9564ba5a8a0103bd7b7506dd74c19f043b5ebca5152e2952" },
                { "uk", "00c09c52090a3aaa113a3f064c5d07a579779e8acaf1eb4ab6ce13a453b1ca7a4aa9da614be0c0f43abc35b00e69146cd274e35004d11f4b46ebd6da79f4dd77" },
                { "ur", "411ba4a37555467dffb29848bd3d552b719cfd227f40427c1d0faed8904b97a6d0be8efdbc043405204fc79d6445fb7a47654d59daf70a8a1a9caf15c62dd6fc" },
                { "uz", "27cef695e74743d9a2a6f3ff6b5b7908e20ca77e3a77dc7a941fc4161f387ae388cdc70de403f224dde383c8ed32f4309c7a2b3c0a7f69128ff57070dd4ae585" },
                { "vi", "24f187ef3d39e3af362771cbd1c6b2d71dfc48f6896f73ec63cd16a7c7a33d368d46de16301868b2629cd404a1a9c8eeb82addbb740593a3f7d710b41962f4bd" },
                { "xh", "a44a065384ed9de70dbae91785d4ea621549062b51d0c0c2f9f1291b28b996624fb6b8332a3a1e9c98c5b4159effa0bc7a51e9186bd6d76935fccb781059be61" },
                { "zh-CN", "8e0e35518f2b1d88d918573cc3570debcd9f82cff3c6e0b7e02235a2cd66d9f1be682a4a950258b70914f54ab806fb8a34fb026175d6797af1d56e19746c1253" },
                { "zh-TW", "c1e60e3f5de87b78df380e595c5ec06ac7657d0fde6f9fd768c12b2237227080e41e596f18d39b00886f298a7d481712316df6abca797bbe17c00290c84e8654" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "80e80d815699b0627f3ae785ffa40c2c8809e03635b7d203b816a2d744f09cef7918129074e426db00aee6d5630f2e71a0300c44c8f19e385fa9deaf8d48f7b0" },
                { "af", "fe915ceb0e1b4dbd96dc0c4cde0c9818a061efb1ca377899ac27471f72068ddeae0d5eec7f8cba6b0109922abe4d8a726c3e10b6fd548eac1042f36c5bdf9137" },
                { "an", "9effc464602a767c14cd5e2a39a2e7768b59394d81a9666a33249c90308ac4e7b41a8887a0516577ce4dbcac64280a395540c2d64fd88287ae6ff86a69aaaf2e" },
                { "ar", "0949eb29fd16d52c76f2b94cedb92a160297983969584c7236aab25757208228d804c1c55a45247ab33edab7f6ec6d13024fc0dab860bf2cffcae0e2818922df" },
                { "ast", "250bf6429b9da03aa2898ff37fef4d3fd2f5e285570a62832ba16779f9ce538945aef0a75ebbc9626c267b2cb4526a441d6eda01a6ac2c5982bdf1255854b727" },
                { "az", "fbedc47b882b47240bb28eb94ed2fbfa6dbec83660852fef10babdb1fe39911619e8fe30efa964d615fe3518611f0d7b34c2aa40d3bf43c149eaa3c831322534" },
                { "be", "f81258f7dea4e529553e9382e99759d834849924629e43b433f0f169bf549cb8a89ef5167aa7c2657de25679400ba50bba6fa5be0d6acc2c8e6bc1994a74112b" },
                { "bg", "5ac34b980a2a7d4f06e9875e6920ca932c20eced7962b7ba6221b47164b1e82990801670aaba7dbd8985d81a86f45b31eac4ee93dedd53558c5fa2db769365ee" },
                { "bn", "28de0cd806e2f6d735dddb0a89175ea0cd932c6279d8bed0e2d133f6269f60c47c0b4461228523b24a01a0f140b119ea14c9e2fe595c2c5526acb2c827faf43e" },
                { "br", "6ac8a95732939cb2e419b1479d255470ddc3a3380624dffd6a6e4b56cb68f425239c0d1adff4a9bd494f6f1fa80e72fc110b65e0db9f5437246a98be06dece61" },
                { "bs", "46c50e5366d184c1272d5b82c3f2e2ce8086bb8bb1960e59e5282fe1b0d06adad191d0038103d9b0210cdc75dd3f737cbd5bafa8b29ede438fe23eb5552a1f69" },
                { "ca", "4311457dafef0e083444e5e5e20676897193ac3277ad553792d672d2c3709c145c426e493c9a485d94933163c065b50f36ae33932196c7018e31a112c40a369a" },
                { "cak", "4b47ec9da81086c868888245f2eee0d5d2b56a7f40d99b5956f46a014254f62c0b118c78da99cc318f28613ae03777df87c02fa2e6d81aa0539b50090d4eaae4" },
                { "cs", "704ce3889777fdabdd4dce4d012bb906598951d7e3fc9fdc489f9e1ade1ff269f8353b3ab5bf738ff1b28bd0c1e0005fda3617117effd711261cf40267129039" },
                { "cy", "4c36189821f9341aefe90dfdc1c8452d4014c96dd6f5217398b463218fef9853afd4232036e44b5bc93d14088b76dc3b859483286bb5d638b9140957f968df4c" },
                { "da", "5c17d471120809febcbdb527fec69bf98777de3bc6b2d9f0987363023839e9a0891957fc842826213ed4d31793b088e691dfb6878886ae94dc337b44b205ddb8" },
                { "de", "b0b17345eb22ecd94000075f87bd5f229fe16c2061f88e1c7503ec58a4c24244818414a333c923fd545241164a1430839bf2a62bfffa751e56ef4dbcbfc7e910" },
                { "dsb", "1f0e603b3a737485be23474122fad03d714ed0744758bff85fd505a8ea87d097709ec53e952ee6793d2022d6a68976708922938d5a7835f9c1b1976663dbd596" },
                { "el", "2c3eb5163cce74350e830c9f0a2190f3460d68b692cb841629fdd720ce9d3aaf9c0d628c2ebe07a223e3449c73271d1b625edc15e920c0ff00750a879d5a4441" },
                { "en-CA", "189b594e0752ac35aad98eb402de5fce75bd4949dd85bf6c88fa315858ec09992a45107bb660b1d0587e3236787367420e74496ee4fb474547c2bd93729ee615" },
                { "en-GB", "23e18b2898defc8e894a836e5b8d618a60bad19bde1b08bcfb2b9769a7d464c29aa13ad556e06846d3597e56fa419941ce877fc366293e27933f0f9329449c2d" },
                { "en-US", "a3ccf778417f06eaf2ed9c008b167434f7039e7f26405ba4f4d29de8e3959ceade152f29829e09e5957c75e13a2c405ab2cffda8d96f7f825c013c40517ca230" },
                { "eo", "b014c5a0553a011b93d1dddf1d923ed721579b869823436bd939d81de0711f9c7014c8fd723ef6a439c34fdb0f1c10a546b9adcc233290f7a6a6699cb26a595e" },
                { "es-AR", "ba01fb30533941c841958ba01686f9c85da9bde0575a41ebbfb67481edb39c33b99e3a11b91eae6c13b0379e07ee6757b61189833a12a13ad8fa268d66aaba3e" },
                { "es-CL", "9bd5a82d178e6a30ce9b9476f7e81c86e1f12285a71d7898350371288f615e1556d0995dbc9974002bfb51c5349648f58703e046da381da885a3704781e43eb5" },
                { "es-ES", "7c58c85b34a1bab1a5a865cfe257018215e25fca93604dc2e83b38d4391e1160eaec164c9b3a4a1414b3e7129b3786a571c7bf2065afd8469447a58f5960e766" },
                { "es-MX", "bcc88e323c1ee10199d23fe901a6b03ee9f1d0a0693bbe586ecdb9e3c5d64b57a6450b14cc1e15e05ec902272c4d1f849e3a91dfb3445c61be2d849a949c6b9a" },
                { "et", "f51f0265f7f834f7996156258bd446fe6ec4982be5c893f8ec0342492d9370d366e8ae87df4c1c0e300a968c78c12441103b582b17e8f1be2bd57fbb66dda579" },
                { "eu", "5b413ed0ce03fdbaf6c1461c1c03ca3e9641766bd79f678ec2b0cb01fb91ce4273e13e0cd0435113311853cb80d93a79d446bf2dc9315cf1a3494ef9f5866570" },
                { "fa", "ba0a831b117a5d868850a6ea5147ea8867a754df4d61d1af4c71fa665191541408b76cbd03a6f73715c9342a3031ef3c395b8ad4bcc0c408fe530bbdb96b7f4e" },
                { "ff", "cc13ce914623f3b54f7c38cc68da49d88960d51e1c57ec6c106275d2491d4cc29af48b76dd495ddbc8df5fea7a21823b5f1043f2042220c5223faccb2ae1fac7" },
                { "fi", "2d21c4a68ef582af1082d0d74801986512a44087acd06215faf57761d416742587e64d55d55a8ec12475b2e77135269973427f3609c440352930681d60e38211" },
                { "fr", "3398dfc8e9cfbcc8c60c578892e9b0ea9e06f2852e23d9c7fd474d066cf9515de69b84a3761a3fe778eac5dddd99ed55a1b8d306b8debc5eb3649c3f244a8a80" },
                { "fur", "dbc042cf068faa11a880a17f2e18b35fa057f5972c016e5f0c96de20f5e8563f64566c24a0ad36a386df86b16eb1e00f623dd5454fe34bd332f7732b6edfb95f" },
                { "fy-NL", "cf6bcb5cc8d49fabb5234e3493cc981ff1520a2b4d47cf24e6a97b9232d20fee649b0f38ba39f0836499fe100bdf352d8492ca5dc18058c662360ac9022d5594" },
                { "ga-IE", "779a760e1dc480d82a9ce1ce253223d7b578a113d677474472104748fe75f4392a6f7e740aa77e651fff6bd4cfc960912d54070d704e678d49e577b130832e22" },
                { "gd", "7c15a3052d22d8f85df10313e5af9a359e0f37123271f147544408ca5ab6baeefa4178efde7142a30e10e14f26c92e27e0b03b62bc0ac88c215e59a243d23bdb" },
                { "gl", "b93ab7d9387aac5cb0c98068b57ef466d1a9d3d732995b21d612fcd97cb56361a8081e14067ea27b558029d81b0d703128fd5432a644141416e8ccbf7bad4e1e" },
                { "gn", "517539bfbc484b9c97e867fc3017fc1a34448ac14f376f79bada0236fae108866715bd4f402f6286e1fd3ef5fae6aa140e45325abe4b997e65d6402a7cadecf0" },
                { "gu-IN", "f0cbfc1b8384aef33e69b2ec6ddd637b3f907e4799523adcf094b8d51a7e4bce83a39cf2f25a349b7d6e6917f767f55ed130d7a80edc44fcd94665a453560a6f" },
                { "he", "6640c73908a7de4fef74e2704fe4bd16116f7ce2586447052c83d3ddea7183616c7b44091ec0c9edd326b61944231d04ed1f9cd8621c30b5b1a4d370600dad5a" },
                { "hi-IN", "6a75fca766d3e1f340148980592853b1c191535e9229caf6b70aa5106c63b25e1a76da56150e6c794bd6e69bb0609420b15a5f8973ccc2b9e7e5a6fc675a485c" },
                { "hr", "86c6d1d46e4c3be9be2f06172d7e4be012537705b217e6ee56c9e6b826fe925c5338b23eec47610d6584ae9275b7285dc75245c699904700561c3966ce47fa59" },
                { "hsb", "8d55b4620105553c54e891519c1262d4ae23d456ee2ae460194deb6ad5f0bdbd5179756cab73d77b0a762e79bbeddf2c9a39226c9841e150bf7c6c217b4a90dd" },
                { "hu", "a82e6fdffd74f1e75d5aca21e50d708ad60d18d14ec3cb4a3e96fbf09ea29fcdd7c75c021bc810b6f742a7895ee20d4086e28aa66dec55f018c10fbc1ff8534a" },
                { "hy-AM", "554e1fc07140e736aa7938bb8e77542effca6baa118c3944ad7fd41dffb591c5fc4b24d0e1d21d4f8859379ed11cd552e6ebb4bf0cc49b325c83e5e94b8d7e56" },
                { "ia", "42fc2fc1ed316b36816b1ccdd08a815dc7abf426a1997238e59097a80579e5a9ea46ce18b06328a4f3af8a7f239dce5f077f4ec289cf15baef570c6f0f47e668" },
                { "id", "bc33d551291b5d0d813320abbf6b6817c431fa286c4ee2d522a2841ea9aaf5d094ac45a92e4c2bdd1c82b1ecf442d739cb15f60a55c4f2c5caff1873c884473a" },
                { "is", "3ba617ea4289b3ae524e0e92c2ddb4f3ac2aeb09e20eaf0ec62c01a9052bd6e9c6bba5798ff19033ccca45bcbfcc3ac2058f74ed23b8552d66603f928ed01bbf" },
                { "it", "31fc7273b712cbdb4b98d4228cb97c4eb9c17d64d8e4180dbe63efc3590254815a568523f3d3f17467cad6c89a4b4d04f2014e65f7f557d36b5533fca3a6c438" },
                { "ja", "7f7baa7c601d939b906e837dfa58001949244ffd808447b3c09b148e89bd22eefe5ef1096d0a389192c69403eca4b197d9155aafe034ad330b06e14e5bb3ae05" },
                { "ka", "5e0a1c9e0320cdc9ce7127e5c619b157cf6294e2d68afd2f7f0dc22e7cfb3fe1e89914abf3b40379528f3ef6f6107405306c6945323faefbf6bf6156e7e2bae1" },
                { "kab", "c0efdff64bdd5f55bfbc288848597990dea9d23f85563a38e73ca2d4682bc43278b9991eece0ed04388284ce7c4ba720c8e10681a3b39c386b5a0abd7559549b" },
                { "kk", "014533a955a1590614724230b98c4ec5b9561db297e1e45ed0a7634198149276b26791a5fc6858db3a01646cb8685a1a5cee438abb8837d16fc9883431937952" },
                { "km", "0b6071bdf88a3602e3f53052a998ceff30a50407c03404385436ef307b28e78713eca13e725d06d25e6a47a9a0121761d56d37a20cd4a1c88814e96304f90894" },
                { "kn", "f5d3c5cc890115e0268912c3818de5f7d7fb33e77f608ef1b786dcfa85e8bf114f6fedf5a2bc7b601acaceb4aa08637eb6e3a75dd032a66330c62060726707a6" },
                { "ko", "4afd53a668250acc54fd690105d274e8d6373b36efdbd897d48cc5782c54e88a271a0a5c3924b29a680082a15f636fb0547cc2217598d77c8240b7cbe99de47f" },
                { "lij", "e78e35721f878f2ae570fcb75e9e04078d1b611854310ca0ad7f2498b1ad6f5877c1d9812fd1842051c75991ea285a8a926eba7ee22b7a44843d4767bf7f0d00" },
                { "lt", "643b822fdb25698ff3944e4d143be21a43c53426b33928ed53c6804ad91c401cf559fdec5aa19956fa89e94c22aaac0ae43cf71aa198b852ddf4025ff559d556" },
                { "lv", "bcff6b182b71cdb5a91553dc0733f8573d4ce033b2bfdd3463f6873e65fd616e2257c7d250ada7180c1a56865a902751829f366e43c42d675eebeab91e0b30f4" },
                { "mk", "c3fd5c1df607cb21941b03c6f62c114bf151a7734d5bedcadb8a3be1bb43d0d7748bcb11e02c368b397c1bb79f74ca630690f3c7f23189fe232e81c5a68bddf5" },
                { "mr", "2ca44f048f3b930ae3aa88953207a88e3cedd7ea245e9c04c97baf0cbf3bbb895b8812d9ea97403c4a7e66b283bd3ee999afc6c285359fa4100f037c43a9ff79" },
                { "ms", "84829d7b08ef122232a53e7fe17e8e41373a42583103a1d4d9a699c076a67b1bafed1892cb00c46f1bf16adbd56606fe608eaca47a9ea06fc08260f2cd9dbac1" },
                { "my", "c32f7815ad7062cc32d3911f787a2514539a3f9a8dc32d09e0265c7d13ea1bad8689c04291bde5c7626125d73573e368fc9f4d52878a20c76616915bdc9ad332" },
                { "nb-NO", "0af9280fd30958a7bc1f5facb3246f05783f4e09c46d9579695a230a1c462b0977511663bc745da08888d1f0cc3d1302a53f92a07fd07a4a76fbd38c8b6aa4cd" },
                { "ne-NP", "f77b1ff18645b223219bb98ad62f521e0ca1d631d2c2f9f3f28f7c75feaf3300f3a7ea650dd4e5128374ea4f8d5f08a81f971ea8efa524b0555cc19f726e3c80" },
                { "nl", "7196795c90eb43f53b107befc40ddaadc4894d4c091d94686183f999f6a76bbef8a05976e896f4a54d37653b549d90153a31cca4fd14709cbd06893dec6d0d72" },
                { "nn-NO", "1434c8dbaebc09a9707a3a5e0de9b0ef87481d26af4e3afbf4b39809d67ee27ee1663d1f443aaaef46f4a57c158c7a086a8bc6e4108edeb4b6736567279a9d93" },
                { "oc", "0f1a3004c9579b622d1c6d2bb77dd6ebd408c05096a92dd21b19971efe6017c52f04a46c539c4a4beecd53dead19c57dc7e1da2700cdf6f987b02c3784f82239" },
                { "pa-IN", "42d1b79d963f5f9c404cfe29324957815b4c43f6d1a5ad213787df6bc3aa20bdce6710bdfe15e0ffe0e9cb6053aca1a5ec355149234b737a250de20078c98618" },
                { "pl", "4fc2e70157f0441678ced7f28fad38e684485e5b5b92b74d6a96462ef2fb056dcb3691dc0406b1a705c7c12dfe66ca83418a11c27ee53deb1b50a912a4720aa6" },
                { "pt-BR", "eed347bd591428bb52c393cc8b15771bf3f8a5a511e29f31d6ea163a9158ddba20962ade23fe68b855e7a65e977aaa03d7e4ddad3b672bf5d1216a8332b4a9fc" },
                { "pt-PT", "e61673bd6d633e7d3e3ebb1e30dde3647bd60e96a318d8420dd2f69c4c0667e5995cb546dedaf0b5225f22d186c539eb111ee5957bacdc4f97ad3bd337ebd1cd" },
                { "rm", "057e3116e23c9ed2b7c6b11a5b5056d08f6f1c6f4279299c547397d90a3d4067459560ef813e782d0c5db6619b504f74a2c8d96da7b22bda777c9f81bd06cd2b" },
                { "ro", "373d228653b1ea998806efb608489ec09f22ba6ea9324e9e1418ee23f014dc37d730b1617cf24c9826ab9f4a46e090e10b413bfbcec6bb62f03bdabdb780dee1" },
                { "ru", "fc316727964693a75c9e7523e7e4158f18173c825ecabcda572ad475dd10801d09ad0344f7e4f9564ec30ac35f285c01302e7e9941951c2b52a8e92ea8d3e269" },
                { "sc", "34d4120769f4513643d561ed48f64c645cc9ceb3bd536a10531b4798fbd11ae9d626ac15d8922aa002ec033a0a7720aa04117fd5eead9a74253403f8855b24cf" },
                { "sco", "71ec66b1899002d5977d447f6178d163eb14f0c126d8ef32228e94e5016728a0882c19cb3b56f25075afb37122aca410a65815242252344280c031359375198d" },
                { "si", "18ecca132f763218e8180a59b9a9190a6ac63445be8f5367b10fcecda49bec60278156e2a5d83e5eb25daafe770521b79531e394922b29b014cb0c297975fad4" },
                { "sk", "9d86a64d1e1e3eb608e58e6e3156a6c212fceb4bf47b31c482398745542a5c9e848e907ed5730e37b1869abfca7adad3388c8603a5a057197c7a9594b1eb6aa3" },
                { "sl", "ffce121ce76bafce2b75a0d391410a33c4e0b10d8d562877b4a301d00f06d408c034d6b38d8daf8964a08222b7869059c2814fcf19b171ecef1f62b866c71ce9" },
                { "son", "d67580a680373fa9719dc306a587cde082b07e9639ce27839c7ffc3fad12c932f5d938cfd37127cd0d638a30de8f24f817fc8bf4a31ba5da5f84d329eed39964" },
                { "sq", "14ef648345cd9c1b8061e3c453b59f26266e9b74028af39fbbb6922ca9ad531c13626d4640071e615192780fcd2dafc2e6fb4a8ae49b9ae4dc435855c981369e" },
                { "sr", "9b16c91b0e27c569700952edbf8ad6f3a40e4db55027bdd63c0cf0a152016a343bace94ed50f9fb62ecaa7a8f259282cf476fb91159722e6828f37dc00335cd1" },
                { "sv-SE", "5b50d9f58af3f8f550d1f37f193f8bbfa53ae150e527c92cad0239f211a44badc3fe21626e768a6581ef3e4d8db728e818239e22570187568252e2badcecea19" },
                { "szl", "ad9930ba7f91992e22df0c022acfd1347afb1646ace6befc624032797385944aef3d80454a6ee66b62efe9284362e6c9df6108a44a39c8424836508e99b081c8" },
                { "ta", "e0279ebce6b16a32a55ea1d7768e4ea1363867626d66e63a40037ac863b16631d609227543c8b35a2c45daea8931db494c8b87c4f1b9856aec6f3ae044cbacb9" },
                { "te", "d7a77ed526dca172c4ea9b3823d6d6999071691c8131abbc7b5c4a774cb1c00179dfe641993e943514adc506de884252b61af4c4a3bc9a76b8b45f2fda43327b" },
                { "tg", "52ca891308cff94039dc9b3d8a78a3249032c829f8e4c83c7e966d3bccca93ff947d0402256da3153b1f8950de0da8657ea0c3d39591c00cbbcf7e3c78532ef4" },
                { "th", "31e6c7fa937e3c6f8e1778b8de5c3db27f257f39142f5603a97033e3bf220476bf5943ea7bee62a2bc9e2108afcd20d6349ab081ae17b4d97c99e368f12f9372" },
                { "tl", "9272b49821ccf258374cb802bc585b9cc806f70816b9df97aa258ae91172fa9d05be601333198c2bffaeaa10f1c1e43558dc3dff5d5c9e39ca9df2fccc9fdc64" },
                { "tr", "387915fc7e89a55feaf6d8861ab1f60876317a728fa1aeed3efc709e5bb5b75901b71ebd6b84bdc6e99ab7712fc31a9385d08ff7310a031d724eabf7a994bf85" },
                { "trs", "d0deb580f84216aaa255c83327390e0cd26c46d771f6b0e7054efe3ef639cfefca21a6d22ea7975301757175d3427cabd729d261c695bde0830a84a050ee2f97" },
                { "uk", "ea44975e01ee5858f119c40a8385272b22d35f60913fa0b9539ac900da89153ea7064a510369e30e92434ccf8764fcb372110db33bb85bb16e15c2f53c2f2909" },
                { "ur", "0bb6e3c45f51ccf30a0093da6d8fe53b0d50ed1a9429ac3d34481911dfb9eabe78ba576b8e737c7f9082b42a8b56dcee7cef8c4dd1e329f810b51b53a0cdecdf" },
                { "uz", "17337bce9c3d39955b35e5a510e3299cc6e9dc277a9757ed11859b7d0d162d088dcc2b517b01d1bda3bcb3db7f065923d76a7db710e62bb10c72d04a9d871370" },
                { "vi", "c86e9d8b945609f8bfc10bfebfc32589f0bce69adb9e6b041d86f62bb8f35222506505ba79ce50a8f4eb6dec0883af13d62358ceb18502823d38c450b4445aa3" },
                { "xh", "2c158e4ccf5f9eea3f465844b263d0e8f392d6ccdf434ee1202b1eb738eb302bcac198d74dad6c3a607999c3983b056e4ee64d63a4f3d77a64b50210330fb046" },
                { "zh-CN", "429cdda31a60ac00a43862b56dc30b9c4305b0601fa24f3bd438e5cc1284c69c8624eb8580c1f61eab057064a235175b26f76d564715e088855547379e3842f4" },
                { "zh-TW", "5fe6ad6106994ae45b73dd27ee12a2f9db89dca53ba8ca3ff965d76395936a10d16d55aaa93cee153f0fa65fef2658eaaa4ddf3415fefef59a39d1d436d40916" }
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
        public static string determineNewestVersion()
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
