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
        private const string currentVersion = "121.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6fb57bbe1d3997ceba6b8f611d88d52b0ad73683d603eb8da5f6029b560507b77426d5cf47174dcc88872dd536beb88d9d9e32a453468afe2ba170bb692dea0e" },
                { "af", "25bddf3601f09af9622034823c3cd82d1fba4bf3f78444597933e51e026b5e0587e7ca131c0ab03aa5a99ed095fec384d889b516f4f0360d5c66cc2caf71d019" },
                { "an", "e873bc9f2593adcdae6446b94f91450b0cf8e0629db446b9b58b2277339ffb9b70566e1920d047543e63cda78dc15db683e9339684eb2838b3ab857152250d56" },
                { "ar", "eeffa15590e95e9c744bbda70e17bd92d2d95247405598a773070092de34a2a71cd5d4bfc8b4560b9435a551e605ef8f95cb48da999196dfb3fc117c1b376e57" },
                { "ast", "4cc193d08aa23d7bf4747c73b225682a470b3eaeca7ff450ca16059943423ed20fc7ebcf3da3326626430d3862d3693eac9cd870fdb79c6ffa14c569366fa70d" },
                { "az", "fbd590bfcc0e4c43b68eea516415331f57c320f97104893aeb81b1f99e418825728ddabc1455d578f8f0af01baa8f4e3737987d651062d3312ab571afeb620d1" },
                { "be", "4a1ef91b00472623f15336ba56bb5b48874c25dd159e0f79aae457b353c1c5df51ace5db4af443f9d139fd97c5a3ca39ec2d663af6545b5de20bcfc0c085918d" },
                { "bg", "426a33fd580bed4aa412320615fab0fa86f5bca13ddabde7f6c7fd397dffcdcec463db7078a46e44642bdce2347783fe0936c3068e4107202742af192c1f8fb0" },
                { "bn", "8bd93c70dfa6f6c7bf2f37a59014209d7a0fbd8b77e55790b37f65bb50d1a53daa2ce0ead6c87c72f40ae132994811d75b63f379f71edaf825469a48d667d022" },
                { "br", "11cae07a7092c37493b6087ab1ec2e2524e5cd3968604a406498ac10da84e20d2e2b0806f1d59a7eb12046c551f119f53c796281814e47c0b849e2f609dcc3ba" },
                { "bs", "f0b607dc8a8bddd5908977cc55fe0209cbc4a1267e354db4fdc26bf290aea1c90ce401bd471b87def94c9be5bb30d3108c9be56e2b560300b939ba36100bce68" },
                { "ca", "63140fa1b7d15c05ef386ef93d9affedabeab5c306c86520d29e9904ff043def92dad0d4b54559f0cd45ce2752c233185b5d99c8861b70f135007f6ef47248e2" },
                { "cak", "67bf54f909482fc96d0e14e7919bdfd5257fdd3eca7c72718a65fd0b2a16686e3145027e8aae12eb31abdfe9b939cad1b0aaeb857518ac3da06c2b011d2a8ace" },
                { "cs", "f1726c586fa72466b3e43e4d028440acb5d01413facacf43c2a879dc4793d647c2cdebfa9293e915254eef41acdb4261c2eec4c90a598c029fef53bc4e7c257b" },
                { "cy", "03aab1d3c22a1664bc0b36aa0def2ce9cf356d5935ac34d39781e5aec77ca63ac528de774b491c5a1843008f7d14cebe970668c2fe81704f421174511a32b993" },
                { "da", "49b1051ae1b05e2f0df2499d608ea0228e341e0c93251a07bde314c75796d6f22c8ed867f0aac5cdb0fe1a5a20664be9158c5b3c053f748611b3b940900be59a" },
                { "de", "9c0af211d1846af20611d05e9345c6a1577ea695273c2cd12f60d5339201b679c5a2ea31f8b3d48aa3303a5c8c2b2cb291180d7def09a38bf31a18654b955dc5" },
                { "dsb", "3e1a99af90219d701e8c85461e4984ab418e364875d3d7e4f4b79402b8f45234f469162f101ca063bfdf0df1a0bf1f24b2a0be20d9a12d1c049ffe0846af6852" },
                { "el", "ad503116b8cfb74f354f4581b13e4e95f06d2777652aa0bb7876f356571dd4ec8f6c73bdc64465d38b3efb7b4cebaf8747e5c25a6dff7d517b59b337947f3672" },
                { "en-CA", "417a03ab9f088e7725029dd10faa93fa0e85ca3eaa6129c4a2547dfd7685bbf53f7f1589db44f71c3dec64b3bfe160d35ef1128a9e898e483340aa0f2d10e0ed" },
                { "en-GB", "bac1f213585f75812872cb6802256a2aaa4f1ada8ad3011a310d01453897c6bf46c1643c924447565ace57341b1d8126994b1c89e5e9ca3916e563703a510db0" },
                { "en-US", "a5f46fc9e95faa9ee3ed9b06e9beab235d68b112c15ee7fe44b4fb2b5971339bb0c52774f3712f23386c66cbae088376cf226bf780bc3bae41edd08093777491" },
                { "eo", "0671e8a6a3839f4f7bbbd48e7535eb7ccb0ef72e253a79228ee8fa508df2b3195eca4032fb202714a87a0138897d78e370da9d69be8982445243b268218815f1" },
                { "es-AR", "c4a4204cd8b3d73b3c63bf67bf0b69938f9cdee047139ee7126553ad0fcf15212d3ead8c8d9700ee8edf467d2eefb960b1483356da522222f2a059fe2ca1e924" },
                { "es-CL", "16ddd08b2ce5a9d0dd66e15f3f014ed41a486a9ab65a4a92e8e0cc472ad704f56e3c07ffd8b8be042b177eab0b66ba14b785d4a95f5bdb42220064c228b80b56" },
                { "es-ES", "1d675542b79c6cd7c830b13c37eb76abfa8c0d1061ea242c558cb41d35646cc1bf5870a4f44b55a60fd3245178bb4b2eb37c4e4db45d64b86deb6a97466df339" },
                { "es-MX", "f2e3a56557ddbabaf74e2c19138c31c7a8908d53278ad4feace7b5496f1f4398dfbbcaf25162902ceeabc742319e4f8db1a414e02022f6b99f7d87bebf105655" },
                { "et", "9aa9f85021af1063ec8a9e39fa1882d66002594e79171cd12675d7b4419cb22311242af0db21c1bb1f8affb64610c01da0447fbb8af3a5016838c55b0b468a09" },
                { "eu", "7f9973df20b5fd55fdaaeea0a58b1f2f8dd72d1cdf037ade72c3a1ce0303162e17b5e101362694a1728e18ce77450ff9d355bff5ce1fd209e0f96d4323eff670" },
                { "fa", "e61b903cbeffd86f26f20cd843b8fe1615d23062cf9b488bd7bba6adc9fdcc11a1adae9b48d5d351b0f9e78b689ad63bf8f724573d12f5d1bc3e234ade9258e0" },
                { "ff", "0afffde12397fb6af02cd412ca1d264aad34f05cac81f7090c85683851e601057c628bead6aa39739bb68749dd72af9e03b93777f204c40b23267e46bb2239f1" },
                { "fi", "dd7b878fa4e25cb0feb32967d4159c611e0643c97a5b9e7af103d871d109a793509862e500d6773362229dbed62f2f1f94b9f5eff698756069abe1e3e0dbe0ee" },
                { "fr", "235aad21ab761d2b7210fcde6bdcfd39b1e78a101ff87010567b5149d257b5930d4957aa5b9c77fa21b5055de313fc9d03900296e6a54effbdb7046867095db4" },
                { "fur", "b1e75e14a6a575bee5c52a36f8bf44ba38caf56ea85089dbf442621fa6f8f7b1675ccf2939dcb4ba274728644d05612c69413b1228de0c19b8bf8518ad26b743" },
                { "fy-NL", "ee875d81abd6673f58e4e083aa96f2d31ab5dfa993388dfa98eb63e760042913ea0865cd6c171f17e03359ed83e816719553e89d682e3c7594c4dabea24443eb" },
                { "ga-IE", "c3c4d485e4875735d6d14f34b3f8c503bf0ffe455c218548ff5858bf1a498902dc2fc13a81cfae9508423a5a9e2b1a927c89dc44a2ee5407228f04c7433cfcb4" },
                { "gd", "a2ac0cfc8156363113b1d44d6fe5abd42ed81b80efca943ae948f8a297397f9b15d8e2d31db127759434eb8a1431df5e221e1cb7dc63c3dbc936af4e8f48405e" },
                { "gl", "99d60114c0c8435e00de343d4c5d63d2066e9f7a2fc403a62b80e2ba19d3d61443e2dac0084df0c1b5e2e483af6013c39f6cff73779de17234f186793e2b6800" },
                { "gn", "e1e8f74fad4a732973d2bee7988ce73c8049973e8a2bf0f0fdfa48b0903ee58ca4477c0a19b6662d045bd7197971d3d2466caff7371a9e5745138b9695bb30ac" },
                { "gu-IN", "6f8560e153cc66a47e9cf37ca5f5fcff79d6e6bcade592a04786f46e04c4a3e3fdeee4c55648dc3abf471c1794580f1fdef41428375f6adf7610b988258704a8" },
                { "he", "c971211e17ffad8ac26ba9b8e8c63b151972b08801b7cfa45e6e931af6ccdbf387ad45fb5740429e281fb23d48707a5ee0f028dd5e8831897023465db3146c53" },
                { "hi-IN", "4e0694e4f6bc7c8955a13d21c023da915b98d48a196d15cf39eabe2bdd0f941d719fe00e6bdf42eadbee007923173a9593c91cf72bfdce929934b27634fb75fd" },
                { "hr", "d70e4598ffe31ac33818538fc56d5c4aa328c8d9050a0537a0f61f7acfd6b11025bc2cab9e981012bc0aaa2a863798e898756e6fd791ca7b0de2cea8d811a9bc" },
                { "hsb", "fa2fd65f9ac776d05672385583406b755938918e57fbb5f173efb07e5f8e6c463b6710430605c9e210c59a45e99b4a08111c03ea27b01d20aba5f8ec1e67ade1" },
                { "hu", "5f58b5a28469645c1e1fcccfc294ed6596ad5aec88a4ff941be335c264fd145b6536ffcf7ff4b9603cec7c1abbb7f269284e4e420f8defba452bd6141d9075c2" },
                { "hy-AM", "444be5dde0d3b0c013e24ed2e73ce8045a2fa9b5b73c1c1cabbd11d5cd095e9a2a3a29ec6314696cab553cd650dd1cc1b170e840a7329fc130c5206b4fd52c50" },
                { "ia", "fa8d511cb434c3042a94f4e682f90071a20beec1aacc1f76a02fa86b02b4c4f2eff5c60aa706f94bec95b0add6d866152291a8f1d76c67085129a381a9bc5b8c" },
                { "id", "cc2ef341b3417629c20ffb770ba7a31895ee4228ac38dc8058e2fe4b694c51d6e99984ea8fe070ab0503d74ff2550a73da9eae0644a1a599251b5c9ae7f2856f" },
                { "is", "5819fe9dcbd7a97efe1bbe2da02bf0b07b4bbe2b3a8c11c96cccf9b298a3d4b883be643d8598cdf99cca49ea45f54531a57543dbcf10a0ecde9e323af89991d3" },
                { "it", "4ba5b8c2a982d2ff2bc0576678c9d6c18539df7b3783724dc58f12a96fe7ee770bc5d17fac3430152a126d89508349633ec1616252e158740d50b14f960678a2" },
                { "ja", "25649239ebba396fc495e53bdab921c79a666db4b26f513ca0c58cc476be2255c9393a40ee2a5ac064cc868bd02013db54c35596dab79c54cddc82442b73e89f" },
                { "ka", "4ce9ef9ded45df6f0114db9f0394244f0ac75257bd95443c6fd6cb8426e3c5fa03d5f528721cc9719c330bcfcae8271387278d81a17e498a8728b38b28ac5575" },
                { "kab", "4ff351e76a5099ca0cb8133b73d397ab912e0c79ebf71afb6b8a71e74005b117d259b5638311796d4787ed2ddfd37a2d0e0a5faeae6d02de06564ff22491ff06" },
                { "kk", "a1d42b971f60b077d634acff0397b1294258957a01e484b48953c32b4e6f6f910271dd803c8995702b24d717d1acecb04d3353fae2e9b9628e79dd8204cfa39f" },
                { "km", "340c3fe5ef79953210ead2b78ffde213aea8e399e97f9fada0dbdc0c71764108bc5befa75d8939383ac4eca482d613211286af4202d98974797e22407cb66853" },
                { "kn", "ff128019fb87895a3e140fd1fc26a720a5ef1114aebd56dd2d7d4f8ee578320bdb9ae1619a6856772010e1c9b2b09b2d9b0e7b7fe9f20fb7378c473c5b1610aa" },
                { "ko", "fa4a4cdb4833d875c586154bb72d0cc60bd7e868041d0f95ee7953f0ed0a1154c6c5e0f3f68188071cd27f27afb1545b577b170e124a595e585a425a8d8410a3" },
                { "lij", "a03b7aa3b24c9d4a3eaf985e52cfcf6fe990233cfde826b5a35a4e1f97842e4eeffb85b7a2e6959414d795fe2d95881736b55db8f60770ed0c957439d91b5083" },
                { "lt", "e53c26acd9e53b3d972f29ef551cd771e11ebf3a106ced44949373252ffaa813091a34ffc8bf4a19624b35cbe08aafad5bfe04d23a90e9035f96b9200eb4ecd9" },
                { "lv", "706f52dc97c068bec1f987b264ee4303a10b95614402f3542aa8faf3adcf7ef6c6086aa6b70f7286a17808e53f407f6120a5a8fa415560516fad98fef7190c89" },
                { "mk", "6288a3cb089c92937364cbf43b77b4a99d17ec9ebddc96ce0a582e8c6de51b0ba925d3ff28ce6cd61e89fd27d77ea97a8c035221867f85b921c62428d670a945" },
                { "mr", "402f9b6246ecd9ef52fc6474979c20d957591db3acc5b45dbc5fa14a87deafcd4c178dce8f050994c7906d59399e844fbecc634154ec92855abb08eb4ab4eeb0" },
                { "ms", "8206e022d3c2c3eee2c6d51ea26670809f704bc0419e3f3400621b57530c4623473568c46c7ab7f6159181444e015bcdd4020aed12a334f5dbf2538c645e0bb8" },
                { "my", "e8bb1e58c6cc9eb7f84d6cade0dea97dde31e91fdb3bfafe4c9e8d1377d76490ac29e9c08a0a020246097f413aa83564ac218b6787a88852053a5189a9e7ffa1" },
                { "nb-NO", "a082c366f13072a7d7b490f5fdc9c4907506fd62af4214ee9be3da9d34f7722364f980204934a979fee9fb269b32376cdc571265ce0d0e6182c5b1dde255f518" },
                { "ne-NP", "144963e4547d0fd8e4b986213e93bc7718beaf91f030d1085d98cb407b438b0c7367ab28ea3b2879d246f5f21457ba758713e975b43a9ea9c6944a8477ba2304" },
                { "nl", "6fd0024ebd9aafbb76655bf822e9fd330ac040b0eee67046732000fcb6286181a37867c2ded260e7d9e32e83fb8df4ab472e1ea947d4a2ff105863554ea700b2" },
                { "nn-NO", "8df9eb3a74ad318b11d723acdaeb792e251118836c747751f072babfc0b6af71df56aba01bf16c1b652f75cbfb14fd5451b9b3080b974e4fdeb5190e03521d3d" },
                { "oc", "4a0b412e015982707d5e358a21473adf525e837e10ed3aff2d0d15cf38ac5cb58f4753a4e90702235cafa9109f55426adb3f322cee14809561efb2720f34a6b6" },
                { "pa-IN", "d79f84f700d5a3b6b9065801c79266b5184f3aa16d390aab8e6edbaeb54f843f622ca1137bc783858c4f4d52bb637af7088b37e43c894ecda7b05849331fa846" },
                { "pl", "66da74ab01b744184a52d9fb4b3b9d52454402c26d81c5d9f73a7512ddc324da6e8bff924bfe4d4bcc9c806c83f86c87f3bbf35d89b2f0f83ad128353bbd8936" },
                { "pt-BR", "35f0b67f9677f0b184e9dfa03195bd3c6627012dd20232a02abab2eaaf75dbb4ef1b685ba2e02597887ba7539981e0b5956edd4758101b689c4658d74e1eaec3" },
                { "pt-PT", "d33d7e26aff389ed35a0a77c0c5b4d97ce7ec8191e486260497a34f2647047b0aa2bbf2166eac1505ed6949fa7ecedfa15df132fc5e4abc1d8215198e22fde6d" },
                { "rm", "aa9c62a7d87e9ad4c7ed8f3213b9872e719ca1d7567a7c499dbf552edc6b2577e878bfe8c038f449a06333292b075d5b5a10ccb956f659cd1600982f5cf74cd6" },
                { "ro", "dd7c3e36ec9a1231980102ff4cf9d0e7a0f99b370a669c2c773d851566dee4b309d10b1bd32432da91f335361538cd54fc515599e207ea35c3346fb321c70860" },
                { "ru", "5a7b7fd0088c1dc5e7d98133aa30e0148830f985aabc1e24a62e179a0355b8e8fcd2085dc18e75cab06f102cdb1d62197109f1eedb229871c51fe7f4d50e0b51" },
                { "sat", "fe664bd086f8c05686bf56b7d3a83d5a0242f5be2b0e3f32d007a07897c35b114215a8da4eade033ffac10d0d122f79702d6b8c8d9418d7bd008f43c4cfe872f" },
                { "sc", "5b464d0398fdbde6f156e9510e228a1a96935bd47716e91bbdb1d3323360b876d88ad9d30b9bf47b5f4e828ca4575d74d071244f0dea713f86b0f2200e514611" },
                { "sco", "29479c04adf72dd4672424dacfb774f15e12dff2aa80a5af0c4fae0c005382331cd24ce95e662d870810f7321158d994826411732c683efe93700761e63d27fd" },
                { "si", "7826dcfd9a1e95892d48934c8198f3b8d8e1d1d30496586fe52ebde034af8b9828873b3d676b790ea7f995f0715d41d544c7e5c44896a6b0e2839d4326296bea" },
                { "sk", "8991c5f0430705648a95cfba04687744bcca89b5cf6ea9ac03f146a11052f77ad789e2600091b45c5559e3393dffc65bee03f6ca533844e903fecb5221224eaf" },
                { "sl", "526bad185c14dcef48213915251cb5ba0cd388c1a74fab3fd921386d4457a15cdc66a87359ce74b50f19a82d93cbbb82afa68335c2596a10a05ecb0378eeb078" },
                { "son", "49f22461ccbd8c5202422fe986648309e56d4050a4a7948f6a3e4f5df58830252e367883b4cc9cdbe705ca176951a07ee044111f022d5ccd7d7b9fd2f8a7712c" },
                { "sq", "8e9a77109d814535d13bacce0c4abf3e422f5519a69fc24f4bfa3600822bfed41523810b93bf09c55507ac1b0266306511cc226124235dcf7583d15696176bda" },
                { "sr", "d89e78027fff8af7b71564a8e481f9b4401d5ad6318c7894bc0c6c14da7fabb3ed611098e792edb75548ce226508d853d741966db7577260bb1a6c40cdd11eed" },
                { "sv-SE", "fc1e5ba0e8fa42bf090efe11e1ff18d52b60cf78c3178791e88fe68d477a9f96ee94c1f69d5c7ef781f2092aa4ac6886e6441c0d4aad1001eafb3c7831a3a340" },
                { "szl", "a9fb52eb1f60d79b0357837a142ff33374cb96764628c06c3344a870db30b6199c6780cb389f8d8cfbd8a0138f817e33a62aee5cb78971ca0d27354b58d56076" },
                { "ta", "2772eb51a68b6f96394e131692504583ba40efd9bf1b521a553ff28ef4f77c49ab627716f7a4869ce76a81c5836d1287cdc89d0c1f44105bddd3d0fd294ca490" },
                { "te", "f82605e66bad133be4ba0e4673f636ee358be1d1935f5a16cb916f67944272b811c49dedf1072772968fc0ae961ee03c9d6dda9580c1c2e21c2bfa8f58dcae85" },
                { "tg", "176c027093bce8874100c96ee3a4b0bd21e63ba76e930e6cab2b4c1f0244691359ece090f674dfc3c34b759c934c91bbd519f8584abd472caed7f02b5fae2847" },
                { "th", "09b4d5c6e6d7a26166da9ef2819df1631b26d007c845906f8f58c9229f4ebf9886eb04e0406aa4f23f7096d69e1cd3bb3ddf04239332243d721478f951237e01" },
                { "tl", "55f5cc728004d2cb2a60aea2728d6f8368dd39513a667ffe55f851b7600b7b4d6e53c84187be4eab8ff729465e9cabe197102f17498784d101db86e4fe263a17" },
                { "tr", "f9bc6292f136f789bdadabbe3e8ec924014aac4b1ac591b58829ea2a402602fffeb00dd7782aeec02e7fde748aa7a418ce29f625be34bcf43c6d380bf21959cb" },
                { "trs", "754e801165fdbc1ce87f94711e5fdbbbd5bb2d20a6d69caf9ef3902f42efc5fe2055e4a2fa00c265ae46722866f07be93e3150cf4696fb4b2b3fa74e1e2f9f81" },
                { "uk", "9835c3ed7af4d224d81bdb119f7be938bc2eb02d21bd1ce2827309904865ffe9a4803a22e22e54eda9726ef97f837032ead8e9538c68008f74de537d4f8c3da3" },
                { "ur", "63d9d3558f5f41952577a4163f3fc623303a3c99d6bc5c4adaf42b06b2f781c1b71577169668990868ea525298d7163046f91de6d94a317c2c8bbfcf3a8a7650" },
                { "uz", "efbd536bd6a68c86ec6755661211cefdc2ca8ab7a6bfb2162380fb7d3ffb3e5897382ddffbf8f4ade4bc90d332961325999a40cfc422d49751aff3635d9cf699" },
                { "vi", "585567bf5edaee7d62a45c46cc862e154fcfcde7b669f7927cf5b23d165799fefc9b8fab7717747edc5aa652b457496e3bf45c5f265d667b4911a516084fd4c0" },
                { "xh", "035fcd5843f487fb6c69787551e971d2a88db09b77dd8c23d186d64d0d5d92c74419e6accf194e1697b48daae5d1f838d7eb9c4a0d4ab6a7a11510894b88555b" },
                { "zh-CN", "4940bb506e19212441af4653a819a40f8fd3387b542b15f88b69fc58e109f3b4bde2eca69258a886ebfa4d9305b75f3e5d0d4fafff230905784eea51b9470965" },
                { "zh-TW", "c36beda936463f3898c1b17e3650487f3dd5cb6f8535fa13b4d321e4fc4b2b5dcde684be105cc85fcdfb812af0cf90781ce851b323e2502dffada7778a23b9aa" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6ff3692c65c62b6123bfc23738b82beaf4e58d2e7cd45249d357434488a71b37fad3a7ba41b75249ef318a3f8b7ab623a7d2920230a447f9a23eb1dd67f8d801" },
                { "af", "6c581ae7b1f37d70640465bd139e044e43a784e51aa2e48697e99b166d77108e18aae0b12c27b572713db8e1b0699738accbb828d4da6c576b9af1009661088f" },
                { "an", "b38be35c479cd8b361a25cb65ef9786c3c17c32ae36db81826fa40385fef3f06b00def0bb77dd201f4c6e523e085e8fd82dc55e47182c2995f49883da04f39b2" },
                { "ar", "a8b12164540468d91d556b29c9466cf9c1f1d0df405812ae4a45e4671d7e4f0e925c436849fbcb1127554dfe20b6b9e1bf12cf5ae95fdd1eed20896081151abb" },
                { "ast", "40e0053852613f1331ea9a2998d0ca8a0fbd9f778da750588575405d0204818be7740c1487e6ed3121e5f96ba67882d148503c35d64e06e282da1e0de5283dc2" },
                { "az", "9e5d13d7c3edd37729d1dbdc2495b3378581cefb7d1f7de006f808b42cfd7d3ab9a629fa78c918da30aaf4f3632790a322bb9537fe6b860c890d3c05a13f3127" },
                { "be", "f7efb3081c628fe47485cdb2c0e8e497e4dec77580148392115dfa895a63af3b605f606054e45e1dbbc72415796edca99134e664c72dfa4e445e31ca756e650b" },
                { "bg", "5125215fc09e78e4dea7ac2f3ea2d683e243288706c008f6b1667868f5cb2296093535556b6ab7ff368f1e34a60e858aafb7bdc53895c011f108b92188f453ba" },
                { "bn", "d7b67dcb09f76c17015c26d8a0e6b240928b4eeb0216da75ef8a70c56676d0cf818ff88b6dadf07d1c4b086b2cc16405ce31d97b5ad5e00789b33836a71bb24c" },
                { "br", "5bb46663f02173af1f120e9f8d765668a686639c8e35c0d789f24a4eec1a9bc738af6e034f046ad1136cc646b4b15680d84bbf52633cc635692db564cc068762" },
                { "bs", "4c57f43b7343ff0083bcbb3ccd057c1d08ccfc8a7497f6dd88eea4d607c2ef9c336ee6223a323d8304c5c369b4d84ac3829b4011d4502a321f4bb1ee384415ac" },
                { "ca", "c2a620e6b8bdab1b3fd84c621bae297f90e4c056f8e1643c71771d03444741d783b6fa2dfc050b15b1386e5fc2470d7b1f144189d0fea2f5b188b56a73c59d90" },
                { "cak", "d9fd630905228cf98fe24ba61a72149c72a23ec1ae92a545624b0d390eeb617d39a50db247450b1bb2987d6c0698c98c586df4757f912df4a1709dade0ef4f40" },
                { "cs", "275aa518ff7998b445c9c92771d7e2a77903457a55eb0cb3be2668ed34ebf89f0ebdab5fd9fb20c2a623bea898d3682d6e39e117efe60afd275f713dd274fcf7" },
                { "cy", "f4cee9d27f07ed584526f99cfe37f48ed28f01b218ba5e5c98cd2f4835b743dfac5a606beb2ef5d207bca040c38646ef447c897d5596a67e2523d96aa5a738db" },
                { "da", "1a204c0e96eacb1a0eb80b2505a269eb4a324c48c00c8a79ea62aa55d38c17c9017017518510bbc73f4c4db8cc9ce1ed2e2faff9b3f87c47613102e340d673d0" },
                { "de", "8813ea712082a1fc23d6d55f8a3cbfb7fd577ae09b697a8386f97f31fe66dd3043814570dca3c0f05b0902fdcbc13163edd1dbe36633cdfdec0ed4fa9ba38353" },
                { "dsb", "e0d5c10a5b7a1cefdbf64145822bdcbd36a2e2b0f0c12668870a0cff0f0171aa007bcac98a6b6ae71885cd4b42be8ae38c67c13a90fa10b977649e7df366254f" },
                { "el", "aeb47811266cdeddc9faaccd0df8b8a8a7488d49de07a62189ca8045a3634baaac0698c7b192c048b7338768ca90dd4ca260b5bfe5ea8d4cbe5d0c6faf87fcc0" },
                { "en-CA", "e7c9c44f3c3583c6a003bce26bf2e48aebb25e513d25469ca7564da493a0b9410ae97e062b25d8fdc9cae343342dd58999231cad33d74f0ffb42604a8e778603" },
                { "en-GB", "7c55fabefa0ecd4ca0ea207aac512a3882e4fb0968029627bcd385095e1ce3850cf4ab112de5b797d926cad686d4f7f996b246f931c084e7b5c3ff279c950776" },
                { "en-US", "6ed8b305ba21d56da8687fcbb64d45286b3a67020033b0147aa1367f3b1cea2cb69a608eb081d3ec4dd440435ed034f0262d117b4fdd02234f1210d4e6cf02f6" },
                { "eo", "5d7509eb32e418b4aabbae0e27e0e0d66a3b6bac63196d60f568f185201e0c4eaad26042dce11691f4ec64014e6e979a6695216deaee3da10cad2fe0169a090f" },
                { "es-AR", "84cc6f3911518e14d8c9952fea22d43c1476f23971c7a445021bde467ace1d0fbb8e2f38755ff94636d5443733f8fd9ce5c0c523c3233d34ab176d040eefadf3" },
                { "es-CL", "b57fb09cbf3d61cd24b24d4e450306aaadec87e7fe3299ae54ae93dcf3a530042744cb91f43f1504854ce476658f477959c013aa132a2df8b2304df725cfb3f5" },
                { "es-ES", "ee13f40c8c3f2ca1acc846d2766e64e12e391e9f9a24f6a5101ab470a765638ddaf94f04d3a75037978580516f0dcd63a9cc4b0fa3d22e8e3517b1a57afe36ac" },
                { "es-MX", "ad20e0dfbfa83c883becafcedae15c135f1ed0e210cfdc29f94776c25b66ee445e2ad2f8b5fccc17b2246a2e6fb75aa453d32b44232ec49e556d6d49093bf276" },
                { "et", "5f30b29f2c6cb9f9e0f0bbed4635d89d0a069a6e98e4b0e3390822053a1420a15a79a4d14909fe7fd769e4f0adf0b2fa57f84d4aa25dbd7231e72ca103e2e302" },
                { "eu", "25fd2e4b6ad4b4739f8075aff44ff3670d74cd7b13f8837fb1628493a0c2d95a730374284ffe5afd2e7e2c86053e7035e25a44ab3c2ae701bce3181008fcb501" },
                { "fa", "de5bbfb8bf7c581db6bc96d97cdcc192da316bc644dc8c0e31d36159c75fac37f182d96cbf31eae4954aedd9a55486bbf6cf2c405844e8a5ce8509dd202fd7ef" },
                { "ff", "afcbef0a9e035736ffb204468b142c8f74bb75cbeba23413a6d882b599897b749eeb5011c9174feef82c39e175b76e4fc571f3818d2da39aa0d85e08c5aca295" },
                { "fi", "670171a6a42865572e0c902b88295ad7c34251b79e05049825f1eaaf52a3bb33c2b13ffa1a65cd3e3d7e1652692bcab6cd21d086c4fc5c0093e9d17db6cc341c" },
                { "fr", "57315ec0f3ca5e0766545038619121c08e317665ef67764dd2f87606261dd88057a461ec416537b59a717574aba2cb625a4003167dcd12ccd72651700a4d6a59" },
                { "fur", "039e52125187f082ac6362b5e87894eee402d47606261dcc36695a23afe37a820c558f30a20d4b0cd55a5cd49321b68f15c57d1125c0dba89a09a9e92bbeea20" },
                { "fy-NL", "a37bcfc7c335dc19800c3d785f6a0ef48a6893f23af768aa96fabec3575a449d916492bb45e1d60d6847a92906e141b2f0197c0e6d530487332788c971af3bb4" },
                { "ga-IE", "f211651bcc9bddc2279387bc8ae4bc749575a706a8223cb93dbc7fbaa31243ae93b7da09b4948804ad377555cc3b7d2707c618766fa2cbb3f5b789a88ffdadb3" },
                { "gd", "940949837ef0923c5e4384c63af93e49e768288763426d71fcdb386739303e215a3478db05b3c2039bccc5b7876f562038220dc9ff608ce5eb7ad94b7e1d1232" },
                { "gl", "2dc2e6f5b7b8c641e4a365fd5478a123471055039b8dfcd2a35d561e04e59469d74d79cf01b0f16537b2457defcbc35e53583d3b32bb108a22cfe59261b9f2b5" },
                { "gn", "97c404ab2736b90a809eadfa5047629f7d0bfb1ed8975be20ab90a83ac494458a7e984b1c2cbdff00fa996209f084fbe8a5d11b2b1c84a873e3d3ea8f6471b6c" },
                { "gu-IN", "82020ca3a99a1951c323e6114f3df72c011c59ff1ed60551aba2360929326b9b0cd7d871d51c53f06f42fb7a6b9f98294e78247db1dd1050db1e5e6670d8e203" },
                { "he", "645b92b04489b77f82620950b86f845eca8e61f5e3adf9d34a2a096b6017c5fc507fc6351765c3fc4948cb63cdd06efb5a1257f9a3d167d3ca09f3e137c7adda" },
                { "hi-IN", "124d6521c173271b6b34294afc3fb8707c4938fd173aa323fc1c70a5c25712dce9edebc62fa2d11133db207638c1e71d49cbd10acf1191b4fe1a51502a5fa153" },
                { "hr", "ea308db9cd98e098a12978d16ee9e44f003ffa884c140beceb223af54ed497e46dfd8a5a32636592fb5b38264c6acc3adef871c4c9c41c5074d031d2dbb94841" },
                { "hsb", "9f0fe82e0d472974557ac71dace76bf54117c2019a513999fd43221eb77e255587c6cfc0150745c147513d3b4c77447c801419bf5143bd5d5929e57a46c4a3ed" },
                { "hu", "c14213488f1d037594721e68001e9b1458698692e011a2348c0bde7309633d040c96740089c13adaf0e48099668cb742fb1e782fd5ac35125440e824a957ab04" },
                { "hy-AM", "9ec2e8ee27f2d5995679b6f79f70e6991aa58be674f4dd97e8a3a3810a3ad6f26e674c8b881b9b76fda1dbabc3f80e8d7b36a9c162d0f8c8c9459eef339083eb" },
                { "ia", "57792cfcd117894939ca01afafedb8e6e865a859d2aee902b97ec1338931ce0f2253bc8dcce13d98872087f5ae4b5d391c724e4558cee2b799572d8242a1a98b" },
                { "id", "cfcddc73cf9f2887462e2d6a08f5db9431f190450285062403459d307d4dd51d23076e7ead7c9e2151d69affc0f61b85eb136a5e629d0b2723fb76d5922045d1" },
                { "is", "4d8ee663734c2c3183ed9d792316e11c8a52209a6d7264b221b585c02a3068ad536925f1412cbc5875ce4e0aae9897654b3d84a6faae41ac5adb1e630effdab7" },
                { "it", "4f32b6f4e788887334918773b8a37d70bf22a5ec78ee43868787a6332fec0c36ced7b7c739b7d7083b47cbd65981aaba2e2f7824d0a7a99e6826438080dcff0f" },
                { "ja", "1ce15ab1e33223d51759ceae9895142a3d2dbcba997f43cab130b6b6cec26f18d8f7a6a98b0ab2d57f2f380117c0a86988bdc3eaaff8a72f97ca0eb976384d90" },
                { "ka", "199cf69ecb3b4ef6167f6c3bb1d4676d0f35028079ebb2fc8dfce1fe9d2ab531c1792a895792912858650ca98bddecf1255fc80db11d8e55033b2d489fdec7a5" },
                { "kab", "9eb7cb2d0cf931dbe73c7ab08efc1ce8c37b3972a5b589a9cc52e0df18ef79dad93249991da335328b7c2f395fd285d01fbbc57228c2858967e4d55cf2882fdd" },
                { "kk", "37c0c11ff411a091c531e0e76a8c13fdc5a97a60f91924529517dbbd7312c714e42b366b0f84d173295865dce5c226e25c07c8b76e7d23bb2f8b1e26e8242660" },
                { "km", "a58c8a4e2125abc8e4b02e698092b4ba7749d4c6978d8834d25cb2202f01ccc855d4e309745cd2e90c4923e82dc9f71c3c38e24975e6750a63dcde029dcd2840" },
                { "kn", "2e3b4c8ebdd1297e99f2518c3e7314c4343e60964bde3c3a36b747da4074641d5309853ef9ab7a16cba93fca73a4c03be6626ff45bd18b43b6545183467768aa" },
                { "ko", "28d458b7ff911f56aa85d9817e58aa0d75c5c54bd4aa01877edcf616397c2a73ab0754075bc4a854044e53656edbecf036dccf1b25d99fa5a76a4ef271785699" },
                { "lij", "83d704438d051485337372ca241c7c884ee61c7428d324d67d356e5ed42f01b667b4c605d3ca5395c13977b157bc0f1d069e2e02eef43072c83e711d458fa207" },
                { "lt", "e2524d8201921614d3a2a42dd85ec1b2f19f3e346f3308197d2a024667c6fcee1ad8620b0d4271d252e47a76c8f7c845a9b5485f6ed2016d8d096673c66553d2" },
                { "lv", "f8f85cca57f054afe92ab4a97c60b98ec40584839b16b3c4a4880a1409880e590029ac0a793ac2e7e703789cae9f44eb5fcec36a705703596d0c7f7d034271ac" },
                { "mk", "79189644af5871390e814330d6152daa6a70ca51b2fbdd62a6091626cd046aad67b98f23dd42d1d610deefe9ce50db65835b6b205ee237ab8f7ab40f2418c5ee" },
                { "mr", "d8a0f74cc733438e4fd879c39a1388eaac045b966f4a8914bfc3b995bd09cdfa5914bdae5c452e643942e5d70d512b0844e3dc2e892473d0c590c903f3efcf29" },
                { "ms", "fae92ac7a9917242d4c25b08ff6eb35c3b0a4fdd75f3dcec874c53f0e93145f0b43795130e68f26119a691ca84d59deb5d8126dab1250da15dad0e7a32cc0efe" },
                { "my", "a16378a529768ac7b32ba0d235750d05145534652d9057198161bc96854399f1f27299324b1f6cecec0eba341575851c7e3783ded18be52e0559765280741fdd" },
                { "nb-NO", "cea5c9c05d7daceaa4d1e9b6d9aecff30a1fc741206132cae56966c43b36313cfa39d6e2d4e7c52fcaefdb2bc56aaf1cf61e98da81eb8bd2672830c09f1e7bd6" },
                { "ne-NP", "ff718ea827ea70f392c97b3d768d8db1995a865112446af939468439d7c98631ef23a146cc99082db550a221e5e9fff464819d48302495626984641d3190d3ba" },
                { "nl", "bf5b4a86f6905c31a4474d42fa8992a8177d7d37db8826f9bf035c668b5732f7ba707cca1d36823c079df9b92557c94bbdd58b5d141840b6f1785562784f9d18" },
                { "nn-NO", "a193c80c4574f199d0f973931637ec6f318a37050aa2e21533752373d09b7c6afa94ed403570a0a0257e1513b05cc8eb14ef1ebed671f456ddf575f1cb8e1a10" },
                { "oc", "923afa2d61180ff7cf315c47e9e8503ffe0760e3b4b01cec8e426e82e04567402e9953d91365a6b46be633a217b32d2ae2775e545605c274953b3b031b060367" },
                { "pa-IN", "82c73b81b95361adcb169e1c54593eafb69e16c6fa8b2f1d702b7360664d79030b3703d68e52bd187bb98bedcdcd6061d23fd905608bca132ab2912c5ec94048" },
                { "pl", "933edce7ccfcb72926006dd1ad1c2ca0ef30228d9d1fbf9bf5e0c301b4c635d98887c1a9e778fcdefa8a3cd0876a136232724b5368707e51c0ad376b4a789dc1" },
                { "pt-BR", "b63f1ad4f100b4116d14692363b40a24251cd16b43ece9743709a0a34b77e3fe6d35ae2ef18629c7f511861a34658b40b3aa4bfe42d3fe080adb591c8c5edfa7" },
                { "pt-PT", "6bf5578af726240b2d4bb4066fc58e3aed944930121d740509ae14c44eae4c98b61cbe2f527a8cc93e1609d0b3a714ec497d354dc816c60b56dc674c8c1c23e1" },
                { "rm", "e25013c009c83d00143cf58a81bbba2d0c6ce6621bbd6ab2ed94d2cc98d6f94eda00184fdf3f9ee300d897924d53271c29395c4b2928f82aea120c9ec577a8bb" },
                { "ro", "b1782feb89d72d9ac4242f8b832ce73ea7a1cc83baaa059f1566a1d6cc0a89cb6213fd585768e3b0b40ce7c82618fcc12294b5959dd195c14a1b39a5a079b0b7" },
                { "ru", "8ef76ccac6883ceefbb1b51e45938a096ad50d2c34406814abb3fd830216a96d2651842ca0b211b07fa7df99117d465aced3cac93a2bc9dc924b8b5a816d027b" },
                { "sat", "a18bdd410692f61a2333f69ae125a552c27adc56df15a3743e11ed3c921ffc6f0368ab3e89680d1acdbbe397032c271f9cc1b7107d42b465dfab4d74b0f7a244" },
                { "sc", "629a3a4e10978d5340432346bfe3fb0c255c4b5c560cef556c38115165d874bbec63c6d009fda67e59809a2b8bd9122b7aea4d6c8834b626833586e29b5fb8d9" },
                { "sco", "ba86f4a0f2334e5b2fe7cc2e40ed0e348113fd53100a1b98733389067c0a9fbfd20eb3a773344f6c83f1cd4a47c3139c754cc44c4a4325aae6b4cd9b098c899c" },
                { "si", "8530498af90e7e2e6b3573c6dbca35f3b0c7b6cc82a6308b23f88f415d99e7cccd6af82423bbdf61173b3d4453c657189b910b4f63ae0053b2acf78d69aef385" },
                { "sk", "eb15e726c5d2bcc9a2f6511f7ca2a6e1a6b125514b89c08c323f052303beb2e8f0cddd32ac866da2428c29d4a285129f29fc528adc942364b962fc6030dccec4" },
                { "sl", "e2a158eb022cda7f96b4cc70b35cbeb8abf9f12356d5b2158471cf27f02a008a116457441f434add43a832be3eb410408f1beccfbcd0d70ca83c6d653d8cd79d" },
                { "son", "4a5a14c7fe65c81558569b829071c2d6e97294188710ae6a5237fc513f076f1ab3cd782323c35a1e6232af305c5fda2a17def44e238366a5453984f33943ab09" },
                { "sq", "db3ca799c0fe05353670ddd8f684bea12ef7476b1c76ef431c8d5ae0714542863d93e648cdb5fad332d79029ecc8c0c3fd5a57718cfef6c3a477aa6ace68b892" },
                { "sr", "68df10dfd3f8a2a02891a8038b8619e2d84216174168b8be3f62e54bd15e7ad7cf7684c9e454cace8dd90081f2597be8c399f63e50b35415e19b8f4de09b33d6" },
                { "sv-SE", "b3994c7ef75d44627b468add56f6c3f0a45d08581d4915293201fe6679b818bec320b89cb3f921791126f3d8945c301629f761ae73dba9b1897153e16a2c7643" },
                { "szl", "55292c4c576b906898d429d0d928c2a55d00d0e384974ddf2da19da3abe35893181445f72307201f9a33239382ba2d2878fa9e713115c72ffb29a8e70f120555" },
                { "ta", "b423998a9e9daf25172107b022472cf45f8337fc78d837d308dd3ca9021949a9c64f29653a062771bd819bed8b11c88011bcd227a550a2b650c786f10e1826cd" },
                { "te", "6af6346ddf66b271137b6be73448e482174e152022c8a79460bd222c1420d4e626751a7c2a3c74a068b3a2803fdc37731de6efaa61928ae053bda0f94c10f4f5" },
                { "tg", "388b93efd1d7d0e4648806747cf5c71eaca0d955d538f8f4e0e9d45a02a2e87baf6c05d117769d33607dfd4874f94bde85559dd68dd998960d7840890543949a" },
                { "th", "9793e5014fb9d853e0afb8b19a78597f3e92987ae70fefa52436d54be689e84650c8e77d7f945db4acfda7d29195435d501f6cd68d05440151c1e197da69e14e" },
                { "tl", "53aefbf69bf43290c88b32b473272ae3489af4fa576d34ae041f3d9a56651bca0d0b3cb523df9e6a356dc79f5fd6cd346de5de0dff468ccb9d310d7476f12d70" },
                { "tr", "85f729dfbfa2199ad51a976e920d13140c60dc6ef05dca99d14e0a27144ab038edd287238891e736f4553c2734e1de35f5dcd96c6698d9103c6c90d2d9f3f8e4" },
                { "trs", "ebf6e0979109c1ceb8a5dc96229ce6e6d0105b74a17371b0bb1e89344abdc32190c1d67acc1561f9ad8de4cec14c1768da50c7adaf2eb9df3dcec215f1222d7f" },
                { "uk", "6f8863e9d180f0d89ba1918acd11bd0332a4a29b0d1e60b61b1548274180dbf247e2051cea47a10dcaeed257e0eb5656c367e4f91c44eedb06b3247292f33134" },
                { "ur", "ad96a9a988e1de21bebe6f2be24cc2bb90baabf3946bcbbaf930a6d715d159600cdf10ef4106991c869ba50dc3feedb8479b736612fa2a49298a036850254574" },
                { "uz", "71e802106c67e904f3f8b9d60e5bdea908103dad9f06b43320eed1156cbb60978211fa00f3989cce65e82b095d6f8cfe6934a70e9fd45ca37d54035195306f9c" },
                { "vi", "f1de6af0b8eab227404c71e88a5f9328805978405210674a212f08f1e1118491225701f93d43d9284a94a9c1d23ad68f1671bc93e219b12d2ae610eef3a3b19b" },
                { "xh", "a193d5e33c76774ea4b52a7201b6de18d48d1aa8449f1c772606d97bcbeacacc831ffaf8240e7b44cb627f1b6d072275aecb718e26eff56097cbb62f5c4ff91f" },
                { "zh-CN", "a0ab65bea06dbb0518f2d9761fed69ac4983883c9dc0859703ac398d2682e0ebf38b473d42a60670e15ccf225e9929952f6d1c33247a4dadb4334dd2ea59a35f" },
                { "zh-TW", "3279b8c1d6e4833675ad34a2f88668c38ad2d13a2b6520c2bb87a038ad04f4faf177f033e816fd7f46d1463dc2feb82e7a61a468aeeb79eef29c97bfaae758ac" }
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
