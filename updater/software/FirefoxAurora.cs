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
        private const string currentVersion = "110.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "78512d9a0ec44231e32e41df41c0e48f6e2d92e1d12c2e4fa6030cb1874ea0f17768436cac7eabd7fa017cb72d406b7b0c432c75e97885e52fcb4521e991b206" },
                { "af", "2fe7492ffd97b9e019f99449f549057668fefb17109dee6a40e7267ac01cac9edffd37ba8e6815a01e0b9f0de20bb5396ecb49c94c504c19d8c334dce271e544" },
                { "an", "a9b8298a8126c173f992eb6a6cab2e9a16222b5a1f18afba172bb28913fe6d93047df54853950ecaf742e30f9feff1a3b789a305a5d45c9394fca922dfacf901" },
                { "ar", "14a099e5384e78001070ad772098ab5fedd443d84cfe424a99b7a8fa4af424631526af9c664a35d20fcb4d4f4f074bf46648634ade48212788ec745cdc178a3a" },
                { "ast", "777ad4df352a07ef11c24c77873dfcba42c7d2dedd79c95b42962428bf84296386a5fec436255c05c9fdf39ceaf25888534dd23258bb6aa6814fbcf1a990648d" },
                { "az", "3500c9075db3e58ef5452842b8161fef16314ced2c0a66081ff3fc2f42879bde1b7aeb4be4c2b92f227d891b56cdfc96ad69f4c36471226887f1c45a0100446d" },
                { "be", "3f16efe177ee2cfeed9200f0b8005708398396449a99802c5253a98385f6d019c135a8aea1d476908cb135252d736d5775b7953c0ea9301aaa54862151a6292d" },
                { "bg", "77d2d84c8e3ad7b64fbf6e20f8f06d4963b97c730feeb8c1f229875a7cdf70db3eecb5c58dbad441e216ffb51923db434c47f6981505637bb2f2d39c20c93c43" },
                { "bn", "518542967bfc2d5869e0ac4f47c6efb680a77ed30583bca2ed915135deb47e99a05512e30f2d07d497cb381ef1d0d513e6ac75593b1b590131b817ba397013f5" },
                { "br", "fbd58267607c54c83a75c9cc8c5c036e3fedaaecc8d86deffba7a8921036c72ab7f397acd17cbe89c12bdcd60bfc6b90404a03f4342464cbf737ae37dd461161" },
                { "bs", "1678dee810ee46a053006ae6c75794656bc9d20ce16041a265e09b6f275546c7c3a0cfdc858ecfd191a9024d6552499f40165c357e419cbfecfb1313f7c35d25" },
                { "ca", "4d44f8a40ec3d6dbdc56c144fb99e189787c7f838a844f54f2675a14ea38a6bb021141807cb0273289a2b1cf036bd57c138f181a0637e9fe294df80c0388d328" },
                { "cak", "36014ab31be314074bdba061a217e7882c28538461982ad3fc19ec3181175e9557c8185105bd1e76f910b98baefbdd5baa8c41bcec57a61e340a36a0fe098492" },
                { "cs", "023b9bdda0ddb8c2a4de20b5e6af87cf7a6cac9dceea4da71b065167be280c51a819aba46c285f10fcbebc741fdc9dd014e3016596339106ff61cfb72e5a8109" },
                { "cy", "2cadd92492743a5a27b815906547bd10ca7d58be036f11681af32e937cfdbbe670f5489bae5c3ca05afab157dc435b63f96e17ba6fa162f92baebf78e07158ec" },
                { "da", "892f5f7a7d0d0ef699571810de7f45d11bb6b2fad9c3910a4ea8859fa81167399c9b0b9387ddbe0eb7ecd9c4678105de1437e768743379e14db1c64e72086c23" },
                { "de", "d11497977196be8f7c637f702c07410094e4a1ca24977a17dc8161234a6b0705b0415e9ad5dfdb5763d2cf704e7c7c4e680ad42d014d8633740fb35965a33d66" },
                { "dsb", "2304c28696f1dfac4bd0902f165d1aa63c032116bd58aaecf07402d43d1f38ed01770b5436748b0eeb24b18e2eccb2c863c03a7cd3078630c866912ae233ea24" },
                { "el", "80af07012868449611cd9077ed0ef3d52b8307e1f2cd8e79e2465430a0a527283d57533193ae5603cb70a3f805432ddd00a3f4cda36f6f9aeca22382aa09301b" },
                { "en-CA", "9f079ff72b3e1e679a4de3d0e68293ec36fc508cf1d1fb0b1694bdc99beebdcdecf6751726b76f36def723d30b19e1f2cf68c4cec5b9c055407e30d634318cc0" },
                { "en-GB", "6f5c955fd11a197cddb9fe3ce012c23becd3ad6473a39123e4d8a6755bb18d4d7501f2b0809a26f1233bcde5206dfc32dd97f0447900f439e75661429b9f3e70" },
                { "en-US", "5c8a5b43f613f862d410f3166bd7596ad484502eb1fcb85b1d34ba0a27aaebfd1c2c9698c11ca39bef49d804f763891920d5de94d726958d57d53ccd9d635790" },
                { "eo", "481a8097d8d355dc9c8cf3398c8b1187283485922343d2d9791149ca4be6692e50059a7ef6f4a947b5095770a600dced6dc456f815ec0f961e84f004b6ac3518" },
                { "es-AR", "824ea655a7997d2f913547f8d5a5489db033020d8a0f4446cf6b3097160d3bc71d235a29124361b427f2387cef039f2df3597adf4502d7941d761bcd857b5a5b" },
                { "es-CL", "cd19f37211ed82be74999a6edb04e755594180bd10ce22243ad0ca914d05ecf85cbe59836a408a411ac1b0096ad47483126cfdf7cd6532ae5cc10e45702e8143" },
                { "es-ES", "30c29f2c9d2d4408b9b48d32b7b13715c2c2a4346f777a6c8dc2942843b9bd5fc651ad5d089b0f68007440aca19429948d27b383fa8391fdd1a08f2d6688f29c" },
                { "es-MX", "1c1d3b0282afc50801f7cfeb1b816fc46ba673a57a7981ec0ecb09cd8a7921625ee7b54bf27463d3a7ce74d2e9e8dd438c770236f4fe0a86780a9be5e08936a1" },
                { "et", "1a449a6f20b3d7292636403cd8b9f3bd9a6d019e9e0fa581a6a4bda7a08b20a320cb9ed0bfc955ab07429d6fcf4590b41b34b731cdf0ee436a80bc751e6f60d4" },
                { "eu", "c94f42e7f537fe950576af8612f952c90bde6d3e81a9d240f7725a4796a0590f0c8730d36b6c83fbe58a633cc7cba92e80afd6ad0de6a1d1bd3fd1fe5009c11c" },
                { "fa", "46a03192f17856f67dd988354591f8903360a32a6f1db392b2609cb1c0b9d8f5d770ed4ea14922da70c01e52ec1eb5bd064fb7802e6fc35232c3103ba5479c62" },
                { "ff", "188ee87d15fd8962f8bcdb55425e9fd9809bed64399577db29491ec86ed0888f00d317121e8084d8d614041b00cca77eac9f5abb492fc8605cd8f175b74ba580" },
                { "fi", "25338d2328e02a8b76770aa318b1b04da1cb9f6fbf69beae9a3085f243b0e4c5dc63858b964dbf01a90fc3b42ce79d5fe8e7e2c6ec2ef53e17d9fb5ba1eb60aa" },
                { "fr", "a00b787906a8513114b5475d85af863536cd4ecda320c698401f79f5403a074caeede5fa13e03ecd070b26a86551b9ddd1c8df82eba86bb938023ae409427d25" },
                { "fy-NL", "ad481c653a45b3efc257f916c1513b622191942db646239964556ca7c5471480b354296160573cce0727dd1f70db0cc063ebe35f386501567d3f578e9186ea5e" },
                { "ga-IE", "b771e69350688adf72216004ccf25e15a5a243856a5c27da16703110a479409eae551ef9268ba2e48bf621f259455e5cb39e3fd9473e165ef8fdf626c85ce207" },
                { "gd", "43f818b06a637188c5342b59be9f6bbbc2fb407a9aca33dccddf1768298380b5c5649b082bd52eb2b957e7726cbdc64d060827ee8ad953d9f8976843e41f55f0" },
                { "gl", "e9fa90e1db9b0f6ba9ada64a6ed9db6fb7dfc1b8de3e986abd8d266ea757256871e81923c1758bece4f8c1273d0500ec7c57c3dfde68a7a88ab3020857fbdf20" },
                { "gn", "227c0c43fbcf88137d7ed35c3cb0a705e410582a7bd2c34067580dc25aff5f16d6866e21a50d8ce7e06d25b689a4dc7959ab297b52795dc7b3c055990b47e6e0" },
                { "gu-IN", "9de4c5945b8bec00ec87403193cbe200407f4f74360d2da029278193189b0c3781241f3e3e6dff7d146dceaf19e8ac68b3b2364220b546a609a70d846ae9db2f" },
                { "he", "0138e10cfac2ef5a0601f09043452712260fd48619479437acfc5117b7c6b3ba92005770e06ca4c5298c696a2c7f8d06c9b0ccc6aab0a7bac2339aa3a2203a1a" },
                { "hi-IN", "72887d23868f49b61b1931c2909a23dbf20047363f16e9f41e52840a026dfa51045205458d81e2dae4d3e0c9b78014e479df4b5e3bc8665ac7fdcadd8ece6670" },
                { "hr", "75d27fe5c48ccc1dc8311ea27fb311d30205f7ffc86684437798e1c9bb771f8f2ec0824717e6350e75debd7e328c44a1b0e533a6a5837bbe1694874907d22160" },
                { "hsb", "7ffea7f282e1130ab7508b603fd4bc178379655773c3df342ad83f77de332314845e262f332eea7851c97382749e031c3b6677b4ad7b9d84af995a21dc72e5ba" },
                { "hu", "b20378f958ddb69d30f5f113720b3eb28435332fbdca51a63ac592ffd6d1be9fd3d3eede78f1e5138151f289a641df0f54ce2f95dfb0a7695cd7da8015359e20" },
                { "hy-AM", "7dc49ebe56b9961d56d15e4d8fb01c0344470c2c8810f1d3ea4c18ef73ca2662a74628a88a1e803cb87213c3da511973bcd76f4c498dcd457322278b8672f73d" },
                { "ia", "3d11c4c2e744336e53cd53208663215dea4ac227ddd39b5fc8947945afbced43fee5c9ca038d807095df0bd8c28e1fb6408e04aa367a55c214dffbe63c623b3e" },
                { "id", "ad721ed8906c43168a675e3b8d0acc4822333ae011e8d1f55e9c93cbfbe844fd3d94728f17e4cf6520fd578e31441683c94874556a2f0cfed0922ad8e510bbea" },
                { "is", "f1c7cdab186f569c7fc6b93348ffe4e6c506da56373ecf861874379b3ec6b33557d058bbcda0bed3f56a49f5bcb548ec94db08b23e9fb29c1fb344f7eaf36a7c" },
                { "it", "11f73bccfa1cf17526eba1c4c68a6dc337f68bdfcc4b40ff03ea9d131f54ebe10ec8299ba23546688fedc1c7d318e24573bdd96dbafdd81b5eddb88d4a9fcc0b" },
                { "ja", "e17b1f04955579f10388174d063f81a4a0a32a0c3521af2c736e735ff321f4ee384664f2f3eddfedc3a68af84f169f9e1ea8e32878e744a8224ce0ba94a7e8ed" },
                { "ka", "beb12ddd539622c0db8e99244fabcb2d33f9c54a82d841b229ae52b99748cc7c823bec6e25f01a1a787b8a7379d6c9fdc42c6004ac685a1b77260203c4cce9ee" },
                { "kab", "cc1309ad42a6deaa3d6d0872afef1a215b9ef2d732def1a8b9479e267bde7dc72b9c4cebb3fb77c266b40d55e4b6f77c8cb3b419788974bdf368b6105d7d324d" },
                { "kk", "f71c53714b26a0b00985458bf06d7011883b9477127fb29f161ea3b2b3c6340b4b926ce93355225b2ec53e4b442eb09a2ea6883084d624a1c49dd93b5104c8b6" },
                { "km", "377bf5f32c5242708f175962179d65585bd798fb5666a0e23ae58269c1015523709ad6b33e2adaa86a77bb8a6afdf26ea1b03c704f54cc082b717105ed8d89d2" },
                { "kn", "5c58bbd9c26cbbd8226e96579bef6a2f25028cc73424b6856c33dcc6223ac15d4036f4bdbc267e18e6c5d8c3e3329e90c32f38018ab5966c598d398bd4e6a530" },
                { "ko", "5b1abf562f1e15060652c3d7ef479a3a8f26a700fbf2fbd1b7f1ea29ac68533e2e49260c75a616139c86c7079fd3973b20cf3fa6d9f513000b0aae88dc6c9a28" },
                { "lij", "548b2ad4aae4cc3748287297541e2e3843553963597b9ce9ae05391677532ae98e38edb8b1c2e61f38f2a4de0f25c2252df6da6c0ee65c8078becf992c306813" },
                { "lt", "819168668d2531a8db6386712033030c9820e73e0c2d490108ade93f0331e679c3c331508a4444d2682130575cc40d9870c41cdc8ef3fa24cc6db11e336d6cbd" },
                { "lv", "856b0ed82d293a2edaf628f84c8b185c3940bcd626395b6b1e598ef3e0d20881872ff3bd9e1e0bb60ebf79f55d0bc145bc860d78eed18162d920f3886a73d891" },
                { "mk", "ddba35c8195805d36c0c75754562c08f587742f42e807c249f2d77d2772c68d1b3f46c7c15dcd090814424661e8e853bff7cae78ee85d6aafb467900c7e953bc" },
                { "mr", "61d9cebde0a588802f36e8fded74d54c38363c213778bdbae3797362bd2e33ce7440f5e13000c1be8ed8a514fcfeee5dbc8a337a965dc362e94f02e36eb67992" },
                { "ms", "ee5a4b27c9435f4ee993b3927b376bd06a990786e4ab9a5f34d92506818a8996f1a989a2c2680a57ed27ccc024f75e3642b63ed95bc2f6d55da7ac8bd660f095" },
                { "my", "218ea2949b54322c12642265a0ec268d517ed515a74b76ce564262a4a1b7ece680f207ee047f08945fa97000c6b62b674f814526865626bbf738bd8ebcc970af" },
                { "nb-NO", "4205cad4c69ee36b0ff07bcfc8de093d583dfd1d40afb0655d7ba07df2be85b307029322464f856919781875e1a9406fdeda0c6b4ce2aef05ef00b0a410ca96e" },
                { "ne-NP", "38cbc5e39c3b9707b61ea29101e05bb077075f048c98382267a6fc118d5ac6e4a19f9e2ef44ae516df3ec1144a487d46d73d786b0bef98276eccf7ec4d13e716" },
                { "nl", "adbf6f887973e23063c1fed46892829c1d16d2a5b6d2e2bcb5d394e7069796e9f50492bd28d2d851426b5aace2e44256f2403feb6c00eaca0cce4e50500805bf" },
                { "nn-NO", "607b95932e6474e9d9ed980d78f7a2aeac19c02eb5860a3ccecf92b88c279ca99f26f7b23bd2af2e29cc64c833013ea1d4380722ebb3007180fe0a1f961127d4" },
                { "oc", "cb8b5a882e4a6f198ff5d0d60d123f52add28e5850706416f590823f47a3fccdbe4e4af64c826fd95db84d562fa0012e60f9eda58df0172940e91fc1273cdef2" },
                { "pa-IN", "8c92e925cfe313715a1b52fad3c19d68126b24b12912f68f41b00eadc47498872da0e2b85e99d46f355a1436aef972841b2ce61c3205a75d43bb0237cfe809fd" },
                { "pl", "f215edd07c3ca32fbac251a9c46ddc1404abc21bebe742696fb9a636627aadc4c7114780b3a6a5fc94cd1ec82c5f2cb185b782664e13a27ecd295df68e093fad" },
                { "pt-BR", "3ac842c44c9c847a92ee66cb8965763f8449444eb11bf7c30e9e1a84160b7fcdb2a3b3a7f478da7e0af59c7633ea74a5f7e2fa130473a4346171dd66e709bd2a" },
                { "pt-PT", "bf207728175ecc9289d9561da974aef8e5750138f60901e237e07f685c532ccf987a1058695848377f3887eade501cfe95e86d09cdc673d9ef670c79d1d1efde" },
                { "rm", "576e2a8c0b2ad08ae73e5690e9e2b569fb6601a6076308fd70edd72438715c5ad177c30adc39ec210ba9381d52dc84142a5b7189b9ca302135df3456f0caf33f" },
                { "ro", "fb0e79a7918aa8fabfd5a1ed5a40bc646403f94e3b57a631bce0e9c871941d0ac5e46c45930a50fa79e8f21f7dec5314a3b937f6afdea4e3492ea920ab086ce9" },
                { "ru", "125e298dbcca20b345a002f97793ca50a5f973b50f02dc112e736561899ab08bc00252e0f3aa655ecba04c87a9c4a5f09a810254454db6bf486ac3abbef3d747" },
                { "sco", "c6b151359644fb1e4a0a7530fa00d223600166466371ee27d571b4f6393bf59258f4a38d1d2e48599d23adc921fa06169aafe4420decf8cf906187986c40f83f" },
                { "si", "1ac3499a35e46a9cc81c3c4f98a47ff60b463414f264195402cac427c757b8d9f591100f72d55fe805245b208c7f6f34fb7d77effa833600c73e21e6fda1e7dc" },
                { "sk", "98c6229c62a1eedf012e510e467dc5683908f3f8682a323c530da030379b926bfff2e3f0025c4f5ab76569bbdc413614c4a86decc16501ed2a22bbb33356830e" },
                { "sl", "09581d21c816f3fa3910ccb54a54e034855efa009268a4f3bfc69ec112bf31c2d1c6a025aaeea09fce6cfae1ffaf6d7dccceca516741294e426e70f81f61c234" },
                { "son", "a91afd8605066e480031b41f268efffd441ce3f805b9a72b3965554ca984a4553e1493f78b8f2d2210fdda35b4155f46221f1fd94e36ad7f7c307c4f05c2b1fd" },
                { "sq", "d92941615f6981e8c81338b08117b6c128dc0f4afd5101d95ed68b364603020a6196e972588e4858b23bfb4c8814a582fe55bf04d5821ad7107fc0b5c97ec22c" },
                { "sr", "e6a89a56338d06eadaafff58cd4d4805fd86b3f26f35e0579c779489253b06150c4e97126cdaaea5cce621559e5701cf2c95579a2d736b39b624c5ec62b2276a" },
                { "sv-SE", "442771533753a64a48583ada5b417f07ae241865ed992cb0ddfa2df1d2ddf59d9d5f8bf21be59a0199961d850e23dbe0587161d66c6567013fd98a9063380e61" },
                { "szl", "8018b52e5214e1cd97b9404d6de6b7bcba52621cc151928d43805cef9e19535525328db786c22f6e5ca89093d73259eb67cfe3c2853528f67d105396fd910559" },
                { "ta", "ee935752ca119b74e44cf5154646cf112cd8a48899512475d693df659f23befd1f241b2325ceb110474450c9c4f712166ae6b59bb7954a83d92035a6c19dac36" },
                { "te", "68e382c0384425b0405632e53dad05bc59a46997065563685927e93f51e1491383ac00cd28061bb6d74bce122ad48775e7bbf8bf83a6ef7109c2965c726616a2" },
                { "th", "b7092ca37224e325a52f355fd40d7d8f2a53b4e0fd47f714fc48c958d9a6af82c611779fd51c9d7f526dc9e3aea972cb3fc531c53a573a6c141fbf6a26cb7b32" },
                { "tl", "7178cc41050f610da505b4cfa74b682577353683fe740d6c98628819dcdf5af15a1a5de81227887d6fe1d23d800ba6949d9f2a4bb444600575a3103edf012ffb" },
                { "tr", "27e34d67cbf6efa4f04136b3d7f180c01a4a3a97587dd138d59d6255ad797b16418664f5344cb7c94ee86d3439cab37679c66bf384fe616b3bb5e9e4be6d8623" },
                { "trs", "e0aec31b115ae1ce24b714836228da844a94a3ab978298e07fc7a9f3094a3d02b4b1d605358657a49f9347b928397aefdc7b0dbf5dbad97d6dab6b7637fee05e" },
                { "uk", "199657f19eb82ff184c52b0cd13c21f07f2c1cb63778a102172f41e2b42d4ac4fc822c588f08452f94d384c241c2977d59d1fd9d4b80e81bc3285355a7e3b560" },
                { "ur", "633163fa6c55624eefca1f20c46bf6773cbbf9066ea1667e3de9cff9487d3d9c224598a252f7867259372ffebea3ad82527a176b451119b6d590bd59b338b903" },
                { "uz", "e51fc397f38c51f78ac9f533a4a334f6c6308babcd14a4f56bb17efd070a115ffec68ee98f4a1f6dc9c34df3e6a5d508dfc8f8b093f32ddb05c3763525d32982" },
                { "vi", "ac03a2a70ca614330e3141ff0ac41048bdc17fe28b556331b15972f570b712a0a4bde7d7feb2cabb3040b2f32476273c636699f610cf4df5a0f56577a350631c" },
                { "xh", "6a6b411fe52fe9322ae8445edf3a550f52a1d6957c0f59657e51d1144b57d4077acf4860e57607631a99938fbb36e65fe9c9832cd1f806241570bf7ff619d29b" },
                { "zh-CN", "e3842d83bdf21a79249e514f373454ebcb90ffc07e05aeb4f010aaba3484d41a085505c314e86b86fa0b9ca28c60cfe0f752bd4ed96cccff0f9442e74d66d39b" },
                { "zh-TW", "dfef73a9206d60b92723251b638117da5eae91dde70b4dd0175dbb576367429ee41711d9e99c599b35569eaed3f92189069fb55680c95a350d6e6b0411499795" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/110.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "104cb3295321ee2d7e75ed675679b9ac0ae726350852c1b78ac4a63f000345695e64d17e19a31fee8104d37289166507df667c507fbd462dded1627d12b158b0" },
                { "af", "34ea67b269d81b29b54a8bb4423b304aa3dda4019839b04ca6b347e7ea19b9014fa554332c5d491f71462b292d95ba104e7c96d7872a1e0091b6821ac7cb4986" },
                { "an", "5a1b9fcf772f862d54313fc5c202208a36e9103ad1aa9569a3cb620a5dc2caeea93a2a9d652629f5d0921a1a3f810b7b4b105034fbf7a07ed64a98d1b21dfd38" },
                { "ar", "aacca97fc43d44e85d626ea0415f8d1e5e28283b042e5927113b95f3374031f8254b4e2b2805f3e75679492828e1b873ed7978969cdb76b2858ad68ea20a629d" },
                { "ast", "55721eecce3abf1a0078fcd4c999b7d2b3a487de93b1392f62e0690fe68fcf52a4431abd600071d43c3fc117e0f8e2db11e195d8b38e1555fb167379a0c03e7d" },
                { "az", "83e5f0d0434e27de2b2ff5f6a07a97522039b73723a22ca365df25a4daacafc95512a0b7f360e2aaa78aed8be8a9a7dddd9bbc1c910c09b1a16af714bb7a843f" },
                { "be", "39883f717f774a0e958b581ffd4b7bf53c93843f25f06108dca215f5b5b76153eb04d511a7327e83c3d6aef15d70b458b2f44e595d2657fceba7cb19cd6e6327" },
                { "bg", "904efd9b595e906fe6ec26f3c23508d2b9a3f67bdbd6e565714e8f2cb3cb3454123a781a3343ec39d903ef008f37bb9addba18f51b4741981ab316ab7d54bb77" },
                { "bn", "b52293ed4bada2249af15a20c8503c8a3691124e0d69e14a34e2ec5015f11f69a5202a6cc26e4d9d39551ed3f5e1891643ad6ee50194b55999721caf09623768" },
                { "br", "466c059b75ac0433a427f72e53aa9ad46b50f1346c84383dbd62672aa8f81fb7c45ffc894be260dc02da4172bd93d67bcb63718052ec78e7399e4d15acc028e0" },
                { "bs", "175c81f70bbf6e3dee408ed25042522a76857cdf2d988597198da62a131d36890b5980b89b28a4e2352e2ec00a4b4ca5479a37fc1892790ccde1bfb8f9f77e0f" },
                { "ca", "285512349de0bf0574321126a3730b1efaa146d3c0edc6356b70a7cb48cd080cdff123a712f511919a5b6809cb8618b0fb72a465a88eaa23ca4fc33525628926" },
                { "cak", "4e8038092a6880aaea11ea3c13830e9b54a47fd648afdae6eb1f05044425ddcee51d211f06b475e454dd7f23931df5ff3389da73b6446ffb48f20cfbcf0c3382" },
                { "cs", "24d25bd76f2fb173f5832c97fd437026c94fc5402552a9ed57c2a7f5bc2ceb49dbab15a8d3c2bf610af2e133cacc13d3a5d02d1fd50324070fb94ad9bd67d4b7" },
                { "cy", "a46a66e4846ef6283a79c7a91d11ce0e4e31e2ffe2a8caad5b003bdc47e982cb7f768183f3a9671e90cbd809ef9c650af743692810f75bd7a1c1a350bce92763" },
                { "da", "e50f15b6c0314ae30a9537250cb3e2b8475f25c18eab57416b0766ec7a991b04193033ea5d25e18fb1555122178a78532ba8f0635aeef672b1d2982a77515540" },
                { "de", "18ed9968006ffa694ba95b7e098fa8bb21c3ecd0f8c300a6a6ed6113bff3732a38323e3917f8a54b961bd6397870a679952c7d2cce195784a80d529258886af6" },
                { "dsb", "6f38972998ec16d85fcdee8dda493ba88e80ec314678a57726358d2c67481f62ed65e088bb22d6bf701edcb3ff4260b543142978104399640bfdb7e9eee7cf6b" },
                { "el", "d5a1dc19e1fb3a63eb8265b6f6b9b501f374a6b933dde27bf0e3094d2fba547e6f1fba25d9dae07734d910d4979070c1121382dca5bbb2b5ee84900f18713440" },
                { "en-CA", "092b4c0565f83e84a1b03979b2b3161449ea092c70722cbeff5a4d4da719a78b5e750c5927a698157eb7b9b6c87dfbc09ba9847644f4d002e11be081438a0425" },
                { "en-GB", "778f16da7962d62df072c6cccb18794d18762595b4e10d6f79792458a42a47ef21f94701814511142fef5daae62d77d02f5d46f0b388cf982585d17b4535ef6c" },
                { "en-US", "fdfbf229f62dfa55d11a661d7a34505a1c177cdfde9f9e93a5095976823ef6184c7df52bd2c579afb5904e33f9bf81ab0ba1514e3d92c9c6fdeff67b0783df86" },
                { "eo", "7d65669c3a01f129da50c00e0f34af63484cd9bd095b35c15909b014a8744e5853d3371aac195ddbc82a71e8bba9c0877bbef597276b749fc4afe8462a0a517a" },
                { "es-AR", "9f75a1524494d2ff654a65e788ecb323b5a29ddfe29130856244355d25758e6c14adabb3800b9021ad0856e6457a6f667eb996072b9366a7d380e1690a847294" },
                { "es-CL", "bb0d952e6670752828c359b58142818b87927e2a2b38c0395e389bd828508e99fa48cde8c64bffe1a224f6d9965f19bc8a28b8a2bf54cc675833896fec77b9f2" },
                { "es-ES", "f6f1875bd283e6b5530dfe6647edccd5603e25bf2bec2974c4fb2cf9a330d06b241245635e26dac84c6f721aad6564925dbcc1fda394d9d00b15609964667b07" },
                { "es-MX", "a28b6ca93f3fdd3595fbe3f0fb896bad86b528d1e5b731316de9011bd0cd5ad3c1f611c3bb9f08f42bd2ae8763e5980b7b789657380fd5b99c676e4d1d2a0b08" },
                { "et", "1552037036a6ceb9d95fca74480c0ff28cdc3f4c470b4ab288ab3b8d90dce2f6f14fffa3f41303a901e8ec7348e7cc5215ffeb5bc2fb7b3b81b25e50e0a3ea5e" },
                { "eu", "a7c76f5aa312105d3c78793d0495a451e5737eedb7d24c5b91eebfffed5105299e8d8a763b30f5c7ff49cee4b407d6fcad1881eda15e235ab3f86f327f2330b2" },
                { "fa", "2ef2e99932f805a4f5cc735b01e3f6df117469082c4bf19ff02f64c827e6bc006860a88b061c32ebdf33a2fb615904369c9562cb83308e2809256d3b7f1df079" },
                { "ff", "c6412f1173e3e8c063b75e0ce8644c32e4803be8e27ff44e9d9f319011f496b74f3fee1b5d724fe009a5cd7367a0896e54479393697def58913e323fb0e1dca7" },
                { "fi", "d5eb977fc3c155f16ae184ccb1bf9c7d6b19a199a982f616e21790b4ed6f04ab60bb952f5f64c7bafeaa052b0df9aedfe0a8e13ee1567de68c4ea1eb9b6d33ab" },
                { "fr", "7c919d2aa36a5854f641550c769b45d38aeed3d51c080c491d18111dad7d77ed78f7435146081e9463e3ba1f56fe8a6bc53d2f8504ee3ea8b67330b08d70e38c" },
                { "fy-NL", "955f089eba2e1ff18c2deadb9d01cc8fda97de8c6f9e90a285f2eba3a0b09e69b78c4111daebed5d86a1c179e2c3873891a079a413184f105adc3738337a9655" },
                { "ga-IE", "0e66dcecf78bba0d1ba5a2e8488b8fa829dd4ac045666dcc6a0b9014b0f512797a857986fa17142c6fad94820d0a215c9318f2a5d2a0aacb38dc7c49cc10b6e0" },
                { "gd", "2cd2a72d7f82e33e59ace76c0abea64164c9d54bd07a2bb8a6beb0c054bac5fdf2aeae37ee0b88acf414cc3221d55092908329e8476a1dfa596e8f13580ee945" },
                { "gl", "23d191198418e2a266266374d0666f7d8f146290e5ed1bf878d161ed7dd68024c0f4ab7af580168d1966b2c900cbf4b48d5a168fbdcc276340dc0ab970d366f8" },
                { "gn", "e9e46490ae4684b6250c927c4516a645d709c358294b23ce238e33e8766c8e197064355983dc732e5cf7b2aa5ca5d42c3bb688869d47e668ce53c9d65f02ae26" },
                { "gu-IN", "53b14a6fac4d693c7666db2f5af57109694af24d00b76d3fa98634e562842af2dc62e32bab58b497d3c757852f30c059be37f5739958332694f7cc6f309ab3cc" },
                { "he", "6ad7b76d2c60c703673e2bd98a7a2d4bb72511b3c8c859f94d94dabb535dff6513970c61d0dadcad9fab6eba4697ddf46bc1591a4127425224cb44ed2d03f3be" },
                { "hi-IN", "95d1ce20430a72c005f34fe97577db3b4214ebece9f4a6fe4877cc4b3d78cb084fc675b0eaab1f7e91f2a86c426d27e9eef5aced99648fa99bd2fefb17e8d03c" },
                { "hr", "b6ee9b32f6a41535cdbd690de9d59363e68a228bafe8ab0796baa7630293f36ddfb58126879a3df1e3be7d905f18b3f859021e08c7165b5ec9e8081a3656b322" },
                { "hsb", "d977d336a116ced7066d4fa11441ceca9cb4ff9eaf59d5b7d50d3e25f74244391c748579d936a9e4d108ef11e97e81993799ca02aed7fc6afcda86d5f1cbabdb" },
                { "hu", "573c4bb1919969bf9213a8702f965e9bce9144088e6699cde88cd8457a90cc832922d556c2ae9b2fc3642a35dc77a72d9fe047efe7b7f8e8af43f8d2c253a73d" },
                { "hy-AM", "e7cf57024609e1f879d774cb2f47c331adb3f56b22c625e877081a21e96f5bd9e81e6e84c76cfb38d4d28fcb4fa040c90ac91bba3f188eed7bc471f71cc55489" },
                { "ia", "42a0f5631590df01588ceb0828932fe771ad599e379abe78aeca75a6a3d1d9eb5572c635886ff0931adb5f2422dda616e7a74265e1a03c946d142e6564da1a2d" },
                { "id", "cfbdb41c0bec145a7de098f101faf9636cd063a3207c869f09a69eb25f2a1f2e8ad9be49e1397a078191cc94a323f9daa254f3c36e2b4dff9c6509503e831e7c" },
                { "is", "ed3c8911edf0ad09c1eef73b5e0571503313601686e2c226fb308f675b22a4183b5d6fab5958d45772529899f322bd125cf2ad460d1388acd32c106c2b4e8b2b" },
                { "it", "9d17f4514f935152b997927c2d9677e1512eec722c8adeac869b239bd3c4dd246109f433de8c6e9590eff2aa8d8618e229d8b3e5825941f02e3594dde532c504" },
                { "ja", "61d74ea25b08539ab4413cc4350fa7397db2799c62656e8b75fb34ba84e735f44a2078be75d4162762ae9b95b50ab4eafb324762474da803de4e4eac5a8c4b5b" },
                { "ka", "d60c2355f8a78b0dbf9e851d6c8c9af184637414f23dbac3b90fe9ef9420bb20b0f0f5074c40838272a0f0ed245ec2fa2124f447865f339e97614f899a8745cc" },
                { "kab", "49453f07b87895c6d6b8a85011d42f3229a06bc53549dd2dd83ba2734547b2dc60702483f2b46dc1d6e23f0a7be229cc713213944e652f4e6dbb4e5402ec495f" },
                { "kk", "a6bcc5f2665fae5415cee7d0539a99b9444affded6a5885d129305b75e538919392dbad33a85d5f65b3ee8bb7b2d5f8f8bb711a0f246b6dc10fce11e5f8f7db9" },
                { "km", "1084ebccbe91f3529cace50a1e42d4ebf6ff4e7de556e774039815454c31365127771875d530f890ec777338231612e0f7dc635f7d9a7d02b058a65c51582d30" },
                { "kn", "9968f3c4793002a2d3f9adfc5835073fdc16ece330a943b7e60d4b5af6f7332158c157a6aaa7ead58912f9150f8bc9e1cc7a5939b02d3fedccebe3f97ad1d32e" },
                { "ko", "9a3c26ec1a6feaf4d1d0e9254d9af0fb32bef9d5a8f5779af88fc2502ddd1ac0476d1ddfe3c82ea96ddb2eb2084644fccc4e5e9d62cef8c800ad97827e04585a" },
                { "lij", "e0c4e896e5d372a8d010499a6516e7ada7ca3da04c25ad120326dde0cca3610bbc3240c5a91a82662078b9e0c158c30dd8e07df006069a32578ff786fc90c426" },
                { "lt", "6da9073ee7ab513eda1cc7698c7eed0297139a3ab7b158a4c623102cf729cddf921cff167b9899e77ad62d50f283fa6fbbcee87a5c63cf00f08a585348f0eb79" },
                { "lv", "ff76b25a6c09e048cee8bc2c30404e79ebda4cb6c4a228683656e99a77c8751e12df5defcce484049bb6865209a52e7c51cef8528a3c67b756453dab4509c3b6" },
                { "mk", "2dc7f1838eaa3d30bc3745f27be47f62cd2c721c32a96c6b7ca3f9beb8c995e62826d3ce8fe8d4e6232957cb237cd8b98ddbd058a875a1c48e754be4e1d5ce0e" },
                { "mr", "deb322e307d04f5746e5e25f020a49869131c2ea0ffed55b05a623535a78705a1a80a07e729e8cf6f9f47f0b7aeddc369dc504af03de7cf64c7d0f42b84664c5" },
                { "ms", "468f4de82079af4a11d8ae03337efadd158a2a6adb1924ebb2176cc05b03351cbc532e537042ad2da3da1982d36758b8dbbfa8c489cbb42d84ae4b43a7c32610" },
                { "my", "6e15f448f0c1c73e7cbca8c5b0b54854f133255cbbb82d07066db74e6e02475ac0f96da9869a5de1e9083d3f798e53e673ecab421438adeb3dbb02f6cd008b4c" },
                { "nb-NO", "0d822d3d73ac1265fd99575f3639cb456bf0b17fc1132887133013cddb50ec684ac87ed017af5f8719baa2250f6b0d7ecc06fedaad45a2889453483b57c356d6" },
                { "ne-NP", "a221737ba1c2912270b4cc1bdf937956f58ea03247c4415cd14c1fcb6f9a49175845cdbf15b3fe194144e0710fb56bc95c3819970e30379298f6d1c93fbf7056" },
                { "nl", "679533c4fc5aeb76eb4bf4d5778d90c36533df2fb30a9b5196f071ff6e3bd1fdc35010a7fea431cfaf7c287408cce5d0068475366792e6456627786cad3ff33d" },
                { "nn-NO", "c50ee604c554522004f559c91523405ba798a71d757f02e0c366e85f418e0690d542980a041c2e2edccdbc58b2a248577aa5fc10f1dd3d51a6b13f702a7b7e81" },
                { "oc", "7f17b89657f69f582ac53b3c28a112277e27b86c32e8fb275301d1c58121e49c07765a71aeb3f5c4e7e179ede30ffea02a7c2f9f9cc1a37c31efbb3bf2ba600a" },
                { "pa-IN", "8d4fe63459bb5f164a7cf32312863e3b93490a800e6638fe90c5280abea8b395581eb531176624d77fa66686e7b7f5a65787347dcb31ddfcbd68779fed23a7ac" },
                { "pl", "cebf36e7ec06029d02e08ba6081d0b75e56f46b0f6757d21e4b24224dd742cddf17d9a6cd5f2fa73b1fb7c7ea9748f3d6c911c04431da82ec1dfb6df206c813f" },
                { "pt-BR", "5204950d0d09ee1e86c0eaec3b6635bec8d1fc72a31f870dafe6880e5bf023125461151cd21f01fd9ec05bd94f0ad4c0d89a5425d64f7725eb4c4f2bce9d76b1" },
                { "pt-PT", "21205bc2c105ec0a38d02571171720f62ddf12e09432ff4678f534ef6abf6ae89c10a4c7461c9d24709a4a52e59955676eef47a3b17f369c83f4125282160cad" },
                { "rm", "35b69be393975e3294d90304e83f6b09a542ecaba13cb24017ceba137edce5d6f52eb33049b4c098dcd599f5b05e1a66497dca53f8064914007586f818c5aa76" },
                { "ro", "bf74b34cf50d8947a5d635dc110188e88779de58e54faa9d8ff3c6b3f28dc9582648de2bbb4df3047b1a520244889bdf3c753f4b26bd0b491a3eecc9bd6f3009" },
                { "ru", "c373531cb9ff6e53cfe5f5da6a253c3d4e8f95f597ae4251e91df4807cb8187ab49081e3fb6eaf32781dd9e73d0f411f10ea171125692ddeb0f98313d564eb0a" },
                { "sco", "99ced0777543722c574a303ef64acc58902f077fb52109c8f6e571b0edebea937650a630f1bc0fe3aee0a867682f209919bb01f904984e5cea7391c8a5906189" },
                { "si", "a3922d14c5b254fe4b67df71d0188e3d403dcf08b5c8cc775fd4f689f1cb1f033af56189f8d9d2666ba8cc3dc6122d96024382fb44f0ff63be5b44d55ef80da0" },
                { "sk", "7c1ef06a4298abdc27e4a895a43c09f2695ba51ef1d07d292aaeb00d9946b9ddad407c083634856aa97d4500ef6324078451b219df6a6997512cb0b3887e448f" },
                { "sl", "540d8cd732ee17e35dc9190817869d1ae6a27b1c40d5be1654c52eee27dc9a48468f9e35bb409ea943c2eab0b9af13c60321093d85b4f285d62772e4c6bab6ba" },
                { "son", "bd1817f916ceaef50b28555aecc9fa2dec9ce62ecfe4dcaf974f1acc7502edbb600d4227e1baa0e2ba464a57e78bad7c23abed837836a00dc5f8c1d4b8ce8ea1" },
                { "sq", "32d9d8808e0bf126462838ce4f533d893772b9a2e22ab2dab9521b7f8f7f9bd06ff9ccdca0b70cf7af7ff171bf933065a446a8fbb692cf53c2d7e5252d096ab8" },
                { "sr", "8bb9e53b799a728c3b8576f8348b0f56aacbabaf9badcc136defda25f29bac608534ce54ec21adf4e38f1ab7d1abc4d93bd80ff9aca573e5daa0df1d50efc8dd" },
                { "sv-SE", "05a17f8983202178bdcb80b664450cf7cabee1a736eb5fd548fbf4fd3fdb0a30ce6dfe42a2f25bf4224eb164c277d63f22c86ffdb10ff22bb01233ba3accc410" },
                { "szl", "b29d38b1ce4a86c87ca7183c71430592d584c5cfab73494543368d74cf4ffcc645381d36709981fdce47ecb21ec2442374ad86d2287fa506a1d4d11998c943ec" },
                { "ta", "db61665bbe1a6d6530270663d092e86f04f350e57a97dee2c1a3e14edbb6e42ea4a71274f882ffa0cc4a1ea2d3517100fcb782fecb69c9ea09523cbeb077c37b" },
                { "te", "aec3887cd78cc6a854c18c04a43f758841ef0c61b07f9a22bc920660efd94fa52a4cd92679c8e2b6b8dd39569cee8a03d0383c2afaf710c66078972baec202d2" },
                { "th", "26de73d80f26247537801812fc5b995d6197f5a4c0e9bf80f232a1a7ac6557ad88b7937d73d69bdf7e5c0722c6d2b2a4f222d3d164e033fe709a6383c3cccab4" },
                { "tl", "1ed8cfe4eeb43205121e188c2d9130dda80e3fc03ba2c60dec735d22e5007154765c57070973a828b5c02b09cfe072af3a8205dfcb63b2440f09d61cf9b76568" },
                { "tr", "1f22b88d037e23038b19192c0e96bdd73a58351169b95474f86b6260f99277fc277f461dc232f060d5172bc37f3aedf106787d98d72b17911e6c8a68768fa743" },
                { "trs", "32509bc676b48c744f3f7f94b94d54cd7fe20370f25b68278ff5640c683215dafca3f4d0b889a02a6427f6d240229a61880c6aedc6a8a16a9e098f4580ae8ab5" },
                { "uk", "cace175e33c45388ac6f333af2eb2516afcefdd2b6506da25e21ab8b707fb9af5427a8c5ed8d18cd301a87745c4bdacea452c8a788e5d0dec96bb958f2aba17d" },
                { "ur", "f4bcce2e89669f997b38de2cf8cb41a6eab4475f8fa92b9515a1f5e2558c7370c017b0602b41b27edd4c08e824165898732bed80b1a77fc4ee9b5f33aba98d42" },
                { "uz", "b74043248af248fe08ac041dbaa1bfa4c442a5900e4dc1fa7681649d34fea497045c809dc2074b34bc29791c64d41938fe2eb13cc215b48d9e5bda40773d1a3f" },
                { "vi", "76ea484393390f7df41b5a2e8cdaf66da7810a69a491c0c3122838c5eb3b4fd1403893ddbe250db7d9e5b9699e79313a55230c771fbef61466751ce8b2c43020" },
                { "xh", "55ff876b445c339193907db7a4e0b4418a29596cd9855dd6221cc0fd0c8aa632f4a49f39f30464fe8ea01abb8fdc998ab745c0dcbe380084101efdb1493b6d46" },
                { "zh-CN", "016c76f3cb48a4b97e75d5b9f4de61bab31a1bbe69c00d8d54d73a0c9d0c2be7a39f4a9c43b25ebe442612b08a7153f0049d3658afb995f9edb3a166ba121183" },
                { "zh-TW", "108aede3008f9d060062d0dd4a7b35ba45da14cfb89c5d164bee1b8559ff13056b68d62c9f0241e705e21ec8ac5799bed1e3870f122e2a0750a62f6039aceef3" }
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
