﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "ce0fb2959d692910cf2cb2d328cb9bd57bb51a59498acc0243371d81c3c33ee395240641232c2aa5af28994d0574eac79c55152fda4edeb5364d6697591a431b" },
                { "ar", "ac3f7049b0486691c8e3749447076c0b6fc8860ace63075e1a15c8ddea5b44a68eb26e863540a12082045ad9614c4975a542ea8b929a779e9eb5a521b0cd7824" },
                { "ast", "dd0dba072a6087349373b0e47fde3344245f1b7ad6e11d44259401f66996de95d8fc71a8e6f097ebbc2a0423f21108681018fbe70b28175fb26fa966768cad36" },
                { "be", "290232270a19818257ad8d1ca2ca818a09e8ce1a5bdfd3e3a349ab6a3c8635935f2be25834efbd0af6727d0e4dd01aa9f486fc28f04e6afe6ffca80e5372b303" },
                { "bg", "492c014774bb096707bd61646e4682c33bec010c84ded943951446cbd9009eba781d668b5b94fa088e6515f33ffa203a50421ac00145b0f72be61a832a8d4b5a" },
                { "br", "58b5bbfe2d10e44a02b475985de491bd36ef46cf3d88bd7113715b4d32c25e1ef7231003c41b830613d5dd701ea5c4a87da1696c043aef0a799359fde1b23b9c" },
                { "ca", "7f4978230830e737754c1fc9165d2708da006693c30075c65c4164d27daffab67a9d60fa77af10495ecd5d541ba3f36988223459a49fe0eb2e7eadad86398ebd" },
                { "cak", "dd4c609057798aa47d03a373b820c4d51d68f3102e99873b2922efe65d29bb5a7e00874b1979d7fd52445ae5fef5eea888cf46c89c3df1fe3c956f3ce8109a27" },
                { "cs", "b9c6d6fe353a805cc50e5950f4f91122d8bbfa4087b43db1da3571c1070544ff7ef000a5470c5b8183c4e6d9d00ae05dbfefaf0b5076d66865e6683f4caea2c1" },
                { "cy", "67ed64773ec1c4e4b86ea94b6b801bb78df618dbb57508ac5e0b7640c68e2e6f599b03544e8b5f8fabd48e221123c2deac66d71be02e3d76a8488e974d52a5be" },
                { "da", "c5513b01a40caecbab08bcfc01e7b80afb5dafabd6fa37a83ee5a02b31c8f9e2fe96fc1df186b554751859df78f875029e2b64a5c0f92f81640b98b2bd733a11" },
                { "de", "f09c5084cd7e58d952aa400bffc361fcb668f206ea8cae661664049be1a00c471598d4b263fb604004287616233e9d594e1ac16c6b13305698e7b9cfd3f3bb35" },
                { "dsb", "6fd7579f105a65f99b3b05cd5df40c4508dbd4586602e1bc09d27372c39d39253aefa7f827db7ba2114cd4b6e62709880a71afc10231c4fac68cd1db8c1c04ea" },
                { "el", "f7da68270ebb6c00f2eae2a308a8fa046ad8eb31d3ad87bc618b73765c2ceb9ba2d8c0b47512f05d7f1a80085604a6c92967d8621e30568b5d0a8f335614ef6b" },
                { "en-CA", "0ad153eb7796167e03748cdea8d5581c3fadfc653df4b811684f5f6743299f58073a4622185b7dc5f971ff4bb6eea10058f6b171f73baf662e006bd7df66b4ec" },
                { "en-GB", "3eeb4a3737735d322de87e0ec893e564a3a2018d6c7299f71f28f9adcc11f7f00d31ade27cee5610f7defdbb0957b0855bef32fbdbbea42d40b5f2f7092d33df" },
                { "en-US", "75b3521da0f019c19721f3426fa9b7381d653533477ef7b244fef25be984f237b2276acf428d28be4d5481b15bf88e3923711e6cc7bedf295e857aabff10c6b9" },
                { "es-AR", "547cd4c852eef5e8336b23a32d73ebc75bbc16546a847eeec8da210fe3965fc439afedcbfed9ea83a83e41319d06b830d67c02884caec46287426bcbca43f234" },
                { "es-ES", "e8f2c36f8dc08b7042f136b42375490e0371c4390f0118ba39c1ec1c1befc21035c224ad3ff735c3e34783647f0f29c5d1bfbe04e253628f5359fae595ffa6d8" },
                { "es-MX", "8a3a6dbff2cb6370d47a3d5a34c9eaf6561e3453e462949e3640ab5a1058ad70278b3e78bb68886ae4253e36244f2c79277dc2c165036a2edc7d5dd264e7480e" },
                { "et", "d304b3bb720770d17b0edf9408107d8921268719773f45cc6060c9ba5d60ac00f7bee565d51f6bb51c186151106b209edc555913bbbd905369a7ecc8d7eb2ebb" },
                { "eu", "7ea6030ccba3d6026ed6e0df887851ede7f954d7411425b46b306e1f759736fd329112aa555e9d31a60a1fffeca21b5ae8bd22cde4e44df142cbf92bfa45ab5e" },
                { "fi", "ce6f3eecf48c9ccf373112f0175d3142c414aeeb6e35a1210f9999f9c6d09d31ebc5e6d70ca982dd4bc163f59e315e06207ec35d9f81b2c54ad313fdad4dd5e8" },
                { "fr", "b9c4d98d772c4a7c35c7d2f1024304506d6b2d0aa52a355132d776777bf0883e3182e607a39d0f5b095dd6a091bb14705313333520c78f70e225fdc5c1bbfd57" },
                { "fy-NL", "b1f6da00f32ff19ffcc806b03350f44f2108f2997c868812bfcc6ef2ffe3ee7d74f972976dfae3b0240789eac85632e00f6ee6ebac929c97a45691fde6240ba3" },
                { "ga-IE", "55c76f90d73b2812b990b65981c3882a8a47e54173f73f394a4c22b988c8c91e23b4b37567e7eeaccf516f021e18b4251d409ff9947fbb22311fce8151a6fe44" },
                { "gd", "9c169eb47079ce8adf1f69722a893f2e0211597edea25c9ca6d4932a0d5c1e2ca76ea7a8ee183a8e85338891a58780dfd9910d036d5b73e4b4e21a3cac43cf19" },
                { "gl", "a81d8cb4d68e9f026cee1d83e34901ed21cb3e2cb4fc469f75fdcc1b28cdee2dbcc6b1fa65ef4ca807b043d9c6838e98aa6c2dc3cf6bd050737f5597f98c8ac9" },
                { "he", "7e8527e1c7c665b4cf35f4c31b9a01545ab4c092af671bf3e2ee6d5fc5ee63a08ca8994c134c564bfd723dfef6f2948deff0ee5373baea8333a84f03094ba0fd" },
                { "hr", "77f2f9d60da43c83341b5e2547db355fc1fd08d0f65c1ce2266cc1ad779e6397721718c4eff3839f9eb00675bb2a1edfb95a84009609fb1597f58e79bb5b0296" },
                { "hsb", "ffb7dd6f78b13ae7401950734c853a6bca5169294d0f38ae9eebcc780e6d58cc7e4536eab89cd7cabfee1b10bc8ac4f4b6419ff61e0febd1a8bc84b9dcf2c704" },
                { "hu", "04a8151bb4d9764cbae48e10e63ec796121cb6b3575f0e2ad7ab63d50030aeef1b5aade1f0d03a4c54809f535c3ca639f58cd3697c770dc517ad636182722fb3" },
                { "hy-AM", "363cf6d2e4910d83cf2402014dd253e6af7ce5ea08975cae16150d8479388679f294f4e3b9b4cabe0502ddb7aa08a3f43a8dbc5d69d67c57df6e1bd976bd4077" },
                { "id", "3e87865f72804df555261713e70de7763d9e374f665feba13d7983eb2f7cee190fdaff1bb0dd9208db93e9979f4f0305cda9a7044f2ee941371b2390cdac3d85" },
                { "is", "794b21241c577ef5d55161f1111d70562ffb7cc1c5824cf295b83c0d5ece39ad0a9060b379a1d2fab369721cb662928174f625d13faacbd216e0fef2ed7d42de" },
                { "it", "af72804cd63b6ea8b5883854e00bb4fe43d171d699c4afd6024628a847ff5537d4f38436de930d0b9f19dbca7a6129c730442284b3009633f03ca8be6813a3de" },
                { "ja", "50376dff1c610a4236fe78207a1e3ffb581fabbb0cad913660d1fec266130d72c93679793964232cc5650fb3769390e4fce2cafdd6cd530fa6e56b344f0f72c2" },
                { "ka", "084ab711436621b6e02d1ed4d4118ec78f722bd88761d2378a08b7d86c13d61a45864f87b7f2632c86994e01a9c83c1613f0da0f2bd1a40ce291d294a6f114cc" },
                { "kab", "51fceb1a7b77087e896b7179553b7d912cb47eb37555b0056306b14df202b348f1d28797d5cc8a4172372fb6e9be293700a3290b502495de169579c1d254cc42" },
                { "kk", "98cb4f01184c2e2e4287e2ecbd87ad47c11a985adef4a7d9b42ee5a29acf0515a09173fe46c3ea956907abecb7727c96dab6f57ae5614fc922311ad43943b5ec" },
                { "ko", "31c929fe720e8ad3e4b6b9cc6bb5f73fec09074dd9ef8171138c8e90a2ccc0d8dca2ca7507661abc13cd1c5f55f93a81b5599bbd06c309ccbe375bc46fadf677" },
                { "lt", "7ac7fbc4f315eca5e93c9f2981cc404afbfbcc1470eda0a93a4731d35225c416d01c4dd3f793433f58a03300ef95eb7781f38cfa6eafd378809048b4719387cf" },
                { "lv", "03d60fd72b807471ccf39a32180772a639160eef8bf2bbdc5911cba8f22c245e12badc7204a4462bcc6c6f2c45ff1ea9ade9b62b737050c80340f0fbc733fb20" },
                { "ms", "e1cd3c92321cce356acd8aadcb2db83c4ae580374dbe6046b60f09423ffbd2fb552c7a3aa9fca2b9e5a6e88ef55e335d49ebc72bfdcfb4a613d26d99289cfa55" },
                { "nb-NO", "6109ae0dc0c42ed7af01cf77caf5ded8015407e2303df4e519d387a7f7e636d509f65e9d1fe34025d215ab65a6eb8709ca20ec10e3962ccbcd6b1e2f87e4983e" },
                { "nl", "6dbf0a6315e5970e3230a6d7a251e75d133ed1aa6528d202a3ec540ba5d30db630108fd70de2809aa38521df99e587225b5f34f837274069241d86f9d999a786" },
                { "nn-NO", "be13ad70b77932c44eb5e41ac86bbd79a52b32dffefa8df0bbda9266306fca6f6a843828a3bed2aa1e975ff660a90ac05f5494c47f34053aa907c9ce154a59df" },
                { "pa-IN", "9d1ff5f8302bb328d0dd9aba1a40c9599ca3d0d1bf4d398c2b611611aed3c9a8a9e4906dc5554eb6bf14af18afa6036b5b8d7d85aa8e4be3d71fa124b2918a3f" },
                { "pl", "47788ac3733c47890d320674f7fe1ea078f29b445c14848c415dae0768980a6cb0f427cc1f79d404b6f414504bf68302fd3df37b9ac09802c79eed186e6e7f4a" },
                { "pt-BR", "ed5d2379c8103d2778994054f81b32917cc85298eccc0f4c27f6fe36c233d36f6fd067a11a2739f15d5b1afafe06cccb8c95b0b8caa0951bd4cdc984a65c8e76" },
                { "pt-PT", "97e3e99f77b81a508368e37dcb74c95e426d50cdd158615c371381945bcbc973598eae93a85881e76b15de548cad95d8ad3add1548a2372d90351c05f390d099" },
                { "rm", "7595355bc05726bb85e001212d62bfd4bd10cfb88df813797390a7245f33f566861d3aff5847e01cd86718d80670a1933649a4092d9f0ced06b0179271e0702b" },
                { "ro", "258f0c615ffcffd830c2319f450e667e8ee566e2ffe1681b596f752b6e8d4e8f37fc160e5384753e1f6a8de40a93ff37ce6b4cc5f641b67f5de988b1c29ceba6" },
                { "ru", "d33203b238c615eb4fa877c03ce486bc05b81ab58ae3d021a7e7f3bc4bc8927a31c1253a5adf2e0bdd1c9c3f3fdfc72399eba6c12f4dcf73e15f827528620529" },
                { "sk", "475c38fb17cada88528015f4a1aaa315b434f2431dc48c966d911fdbea25d667768353f932eb0e7833d6d60a440cc64e235760fd8e4d4ba37370952af8664072" },
                { "sl", "825eddc4e373aa1d51d4ad4abd82418866839bfb7e64fab87cc8047e8ed97aa01c1a328cc5a8203741d2a2873676d8714fc7678400a8cd33d4041384df36a019" },
                { "sq", "bdb35a0e18ca0a814713aa6058d53782e2832828abc6187f3e4e69fa3ff8b5432b4423508613d2e7c5ff42f38cbbb0117a97c601e2e7c4f8ec7fd6a5df114629" },
                { "sr", "9957cecc7fa11b2daa222cb0b37a5c1a59f05cc3a4af79f65df6adb969dee711aa35a168bf1cb4ec7cb2326208976019cc64a900ac80b66d76fa62d75f21eaaf" },
                { "sv-SE", "7b03db1c9bd5a548ad055e80e6d17a474fb41e56fc2ef0df956156aeeca493ed8a7f8e1ebcfc61dd98583b0343964a101179daf9fb8822fe2b5c0d52380ebd9d" },
                { "th", "9a5e1075071e3fa33e87143b3cfb0aec7b9018d54931998d5d318cd6a33ee41caa966c734898b1f0385c59d4f551e518d2f91d9a8348311556ccc5133d9351a4" },
                { "tr", "9c4d597ed78c3a3ddd1695e277d94d04e7008f9e79ef9cef21ff9f2f70a7b8e5020ed5a528453ddeb695a16e284257a5d3bf9d8b45866b0b21fba1d376349784" },
                { "uk", "3f63c65a81482733b664aa2f739f61578e312735bbffa6cd195ad6dcfb3de527a4cbb67075f0c5eac433741bf361d6c46df66fdd64e25ac3546d83451b738e0b" },
                { "uz", "b95ac7619b3dfd7d9c8c82b804059a6f857e4b00e9a50617258e35f13de60ae69d862b359f9c47104c67f352e1ff6b0c0021e1a04cdbc6734efc61c1bfe07fd4" },
                { "vi", "b524c1c36ed3b8226f5743a796b15e01fbe02927399cce896e350bc53c2852bfdf65d75e9299b2151ab3887bcc249ec5764e229ea972ff15cb71dafbcce3c7ad" },
                { "zh-CN", "088c3bbb669cb7796f60b1ecc3dbd6be81f3c47e1b3cc47400f8eacb06899a66f8114748761ddbba60cb36fd9382e5d9ee41189697ec6ca2295a86a6b157a37c" },
                { "zh-TW", "dba666e310bef0446d48752aea3f37f6a2efa171c7250e7a590c8256b507d688214ad45311eb699ef64dea10b2c5088ba5280648f37f7c60a9f13d4512df534e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.10.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "32f54075e1c49f5386dbc90e0d74ee76a9ee1b5a5a3530adf20695d733a7cfa7b1588fefc841a78e8f2be717d5e4b3ea46c85d7606a4facc7d88abf2350790a3" },
                { "ar", "fb5fa9ae015cbe2ae335761be996af50f1374430ce3e74662d0a6e92770ebf2ec18e3e3ba4e0077338c4855194f961aa65d70a444acd06e1ecb66e8489278a63" },
                { "ast", "94a6efb56dd0f87c3dfc7d1ee0bd779376d74c45a761dc7a05df5eb64f3fd291d94339eeee1a8adaca2259eba9da35cb43f3c1fac1526ed958b0bad23eb659e9" },
                { "be", "245715e3c90a44399f2c99cb209889c6132e11bc033df4eadecb7096892c7aeb5f1705ab57f3250a6fd56da6fa8b7d73dd8fb9c1e217261e1e39e1e2c28be6b1" },
                { "bg", "1893af5b481c63d21d4702864885d2f0d98b7d502f61efc7fa1d128a8f964e6a08866a431ba9f793bd6763b74f700cc114f2368c1883dab1ac7d377ee0abd7e5" },
                { "br", "5af9023d55147a22bcd97db58b41b2d661151028a2f650745bf8e74f691e3157c3b787d28be8cf1000d2d4aeddfa635b0ad72f50a10053ff8aa42a72518f4ba1" },
                { "ca", "8644b223e5575890ff7ada8e28bfbd4045dfba1b7cae52f71b4f0694dec96de241e1ddb4f85f4c753a62d56e32be7018e051b503b758cbbd7c0f5dbbddbcb70c" },
                { "cak", "68905619df2299be15018215e9cd149f5ce7481fd8a810983f5962801ef93df039d9a550a1cbd1cb06893d3f04db686cc2ec2b95fb5ebf0a323fedbff22bfbab" },
                { "cs", "5c645a81b8b080e362ecef2760928e55e21ead773abf804b62b4d1b6b6eb862e1cf83d0ec63978014db31e04fd547f4b4c78816085b208a914257007ad384007" },
                { "cy", "52be8078238711e138afb6e030c717a1178b494f61d5abd0f76a4481f8672990b7d1a1946aea87f9f04e83e81683ceff74daefe32495b5fc79c5412c51e380e0" },
                { "da", "1978face78079c2431d6e2fd7fbd4b01633a071c9140bf2eb9ea95976ea2a193cef40064b0be66b1430d6ab4c42e80f35c4be459f020d0d11476cb9cae68b09d" },
                { "de", "57bf52729be0ee76bb82b1ad973609e857538d81b0ae72e14fcdacadf6f28b6161d56592104b0baa730e4dfcd53c5abc2489cbcc953a90d413c735781d2ec8c5" },
                { "dsb", "89001efa6cc0b5124b6bab214e876c45fedff08e69406b6bc53ee21e392fa841c67ee0b4a91670b6bea905076a111eac10c55e17477d5a02b06085106002942d" },
                { "el", "3be58c73fee151c7bfc3d4a887d5b37b4e4d2e92088b7998379add6e6fe58ba502955c4f039e23d292f38389870bac6b6522cc83f78c7692e6bc569816be1885" },
                { "en-CA", "eeb7622e6c37101f6899e68cb77cc33c5f9b5ce498fb9d0540a8f242ce4d81e3b34a8d86604659f71b141881e69e4099d5076091393504c1904d4817dc05c1b5" },
                { "en-GB", "822d6a093817a114934728791ea95f8a27568bda331901957516bef353336e7c692dfb9b6804d268f7bba5bd759ccae3d51984fd7b0965fba270d7f66b6e69b8" },
                { "en-US", "735dfb05e50a99dae77ee43c197f23cc7c30b95287f9a29afea837c8f6348c84ac5b475eaffc91de5db7d27647c9c0c11364058a16e2060e47c6ee12125a9166" },
                { "es-AR", "226c4c50fa6576a28da44313913817ff78dcac8a2efeab815c20a7b83232a26de4eb066369e2ab56b44fcc1174a9867b211f65ad1f60cae3d0ddadfa8556b990" },
                { "es-ES", "6dcf03c9eed85ccc88a9dfdc19186e4675c72700380c0f128622af8e52cb4629e68ed2c2e0a7210b51ee2a1a2d1669af3dfbf6cab4568abebb03ee4c869cc650" },
                { "es-MX", "458ea1563039d5197ff6749c536a28f04e93a5f4a15592a6a8cc6527b1c82b7af4dd3f13df0fa9020ecbb3f45ac82a838e4e63a6e591767c8ba76cec053eeea4" },
                { "et", "773d5a532fa8be79eda91d3be01d9de56213a3ffd600be453bfe44935c188193b8a4ae823045b13b69f267fc56d9f555ecc67f3185edc5769522b2873462f707" },
                { "eu", "e35a1979c4d9b72916577a9057ddedc45bc99c9e2ef8e26d72121c5d6a53c4eece48f46727cf5802974dfeeb862c74fee43921b58538a2c7efc87d352bcfc070" },
                { "fi", "88476bf6483eebf7a0d1548534bdfadf4a3211b67656c8329d1dafe2ce71f915481339ace641bf70a752d5c73d854a47479e7bc741362d8f9d1076984277bb58" },
                { "fr", "8098c32b15156f3123bb88232b20363d4a97796ca33b0ddae036ce4411ca7ff7bac35a36544120b2c1a4df499fdecf08a321a91a052f149b1ef54078ebd23cc8" },
                { "fy-NL", "ec083693afb411b8df5e4c7b1e80cba42ea02b46250bcf08d9d4e34a6d5fd5615235761b00cf8a1fbddbc64471538d962b4948c64d2c6f5b8b9b98b5653e46d4" },
                { "ga-IE", "f0c75b607bc762dd85808589bcd68a8895869cc7558001c607ed182b09643b24b81d773b59f0de5a55389b930a76ab2dab685f01115f1006c495c2b222dead97" },
                { "gd", "b9588bd5f26d3076f097b1014c629dd1222c7bbef433bc64be61a9f7c45627c641e3e44f1da82e3a0bd78d7cdbe57add4930d8f826598d217614f35c0b849afa" },
                { "gl", "ca1fe7bfd6a88af8e1c18b4679cf46e5273b681f66f605f4c60c1a31ab9d7a79fd702ead782a5280e1c3b4a3375e42542637515806fe54a3e7e1c71b34c38685" },
                { "he", "9dbd16b65aaa140b78a68ea44d292329806c075d496c37b4141194e0457378580d606c17264dfd9ff07e82be853d9b377e1e3ce7003ddb701e8e4a3c65faca48" },
                { "hr", "8181a7bd37fb4e30b2fbefbb021fa03a0305e1cf99e75dca214bd242274c8090e2d3acc2683e55947d8b3efb46f5a28defb560aa6a9fa49a5aa96a1863051929" },
                { "hsb", "ed08a0377fe8c97a8e2d40646359b4e14e6504525fef1911232d197202a99aa6e1aa9392c0c3019a67f0ef8838aa6cee684fad3773a4f145767959ff83d4c1ad" },
                { "hu", "a48e5f35c81c62b8154b66b7154150195df7c88a77f75d2e922d3dc784add63454a97ba55df7681e45d3068628fa378028a29eaaebbe19181c8be89723879e3b" },
                { "hy-AM", "b4f2c08def5bfd61024ed169b7d5cde67e5fa9b5ba721b68c964c69c6dfe33f8913020ef5955152ae1588346a9383c10489bc7a7f82add16ed9dee604e1a7840" },
                { "id", "ed6d8be72e85ad32f4a3b76bd27f94f8148a8d309a7bf96164bdf50554afce6d357d7a017a2ed81cd35f156cc58ed6724be05187e5618724058dc1eb8db746b1" },
                { "is", "526de990a902bc5da7b8198269dc1f909dca60dead2d8d3f2c736cea32d90e4b752f80d3ed94c63a6b45a62e5148fc1da66b51286b46ce3c57e98a733c518132" },
                { "it", "8b72caa854a8c1c6bbbf86424cd390e100885b0490cd843d9f45b2a8ed02fbcb04c94774c2fa3265469e694f76eadc4c626d3277e63b7cdccd547cb8f99f4cd1" },
                { "ja", "4a3b8d3437bc5e8f3c7a58f8ab79e8eb36d8509b7181ec3a73a8b25476b2ef9029cf79b0a08e5c0790b2bae884ef2096a40a7c40f46d860532eedbf393732024" },
                { "ka", "b258d7f8b8716464845a2040d10110e1ca91688505a575e7c9b70f97e62ef065735bf774c31ac97e6bbc655a845da8d40aa24ebfaa1f00f5783e7e73666f461b" },
                { "kab", "81ec80909b5a05ab511aa2e8b51791d35848925c3b01b04c1660a01253002395d1c586839bb2ffde2f3d8cdd4b734188690d90b6c588f3e5b9cf9b8a756faae6" },
                { "kk", "3168ec1e2287cc33ab354fbc850a03ea52599dbdfd7f47c86777b34a5501f5927d9a7aef413d2e8b70899ddd0a459ddca258c2bcd1157bcb2fbbba970543c528" },
                { "ko", "e9b2c578d5991b62d1a1a496b7f92bff370bf88d82252dc51625894a33dde09c2e6d90514056db5080e8eb45306e5436d12aaa92ec3de0413b80c1e6fa4df116" },
                { "lt", "63084344475b9870fe0d01751a592335aa40ffeb0acd3f46658185fc69fd9de489051f126e2ec216cc67831ba4c3abcece1f38fb8709d9f08827843405f0cdb5" },
                { "lv", "fc86be759d2d87c033c9ef7d15eb021595869e852fbc3a0089a7cafa441e149b7ab70477aa1233ae281938628f7f44053fd35bfcf880f27a3c2e8c599e439b3f" },
                { "ms", "90f5a18f71018a7381e6ecc26b38fa8111ddb385e9f03a7006261a6d474f244cb34624393700595b8f2606a75bc5d504cc40ac65c5e20646a7249ba10da77e82" },
                { "nb-NO", "c0a1d4e0d013aa64fa4cc83c07902a94a509410f1b2bff4d0840a6f2204d8111c13e9c09f738ea39aaf7a04845fa402cc5c483c6ff7eb801e339ce1cc9a5c11f" },
                { "nl", "f4b56c656b08e74c5250f7d80014814540f11ef40936615ed9658705537d1cf158d0bfb890db451007cb5f4f94564d2d4d2a6ffe5fbe58e745736a7ab5e813cb" },
                { "nn-NO", "bbd34a46b3cf9ca02064720f5a58edef888a05cccc856f43fe588634fffb2a790ee9fca4cbf63a8e2dff7f7efed7f1d5a6259b2fa476e370511b3803974df9e2" },
                { "pa-IN", "b34066fff0e69d3c802a5e36466dc951d78ef6ea60d03e23eb93c5b279602b8edee7a0ff2fa8eafa92984bf0324967ad377c34a7186f6ee8ee866bdcbcb1b29e" },
                { "pl", "63007bf7f54f042092a995e147a2df2a14e9a04d28ec0b7943eb685933a14ba8ac5dc8ba0f27915ddd4b2e0457308e68da50416f11e5a42a8e4bfbdb2dfc0642" },
                { "pt-BR", "93d54f0c9862c685dbfb478270cfe49fed1323c7a445740486651f98a52ee7da87bf3e8a8840a89c5bad4a6eccca25c871d7dffe7bb01fc32d98eb55e0fce0f8" },
                { "pt-PT", "8df69b1c3ce3bd98917779d9ad12434bf77684fb6384bf4be89d47e9b0f2d2bf67130355741c9a9e8a3845066822ce090e9f9a33a313e7f17614b5152dd6fcea" },
                { "rm", "002c9b65267947e7ade871226543b8a48b78c7e4c8545d433e6c70c6c8f487a5b8e228636550c9ba5de2afab42b9ca24d8133cc73bd62fd30a967596ebc85fbc" },
                { "ro", "552fdffa6a9c02ce6f7d10b16e233801dc714d32b943952cb66b02a0810131b4f7f341e4112171a8a101edbf5603b673b27a68806d66b674454408fc9e9fe591" },
                { "ru", "db6225a352f5c2fbc01d76a84d58f9121f978fa4556cf54e44c4e97cdd0063461da79089cb90d4e3ae9a9875b4e8137be538bfd321acef7c11cfaeb34ece79b9" },
                { "sk", "ccbcee443ea764fc0c5964f383311ba11348947705891351ad29bec1804785ef76d5e778badbe94fb7b886d6cd659257fad9222e51990f264f7b2d63f119b032" },
                { "sl", "917902a5b95046c47977e0aa4ab2efb1d524fe1c417c1dc9b63e473e8110edc84bcaf9ce03b9ccbd5198b5887885fa0a286fce7c2d93631628fb422415ad4ef4" },
                { "sq", "9acb5044b4a643fa6fba0b24a0fc917d47389abfe17fc610c8be842247ea78a4a089565991a0c2bef4410f33fab413b7e3f188e9f632fe9e6406fc6550ff4065" },
                { "sr", "80ad8b17fc8c5cb642541458de25b2f0496303943bd44c62f77ed4f708c2e8c6a2c82ad9d6901c3a5ed7962d4aeb7ab3d2954e94363ada85342183225bb642a7" },
                { "sv-SE", "c27eff8e5ebcf4d32c27ca30ec095dda57711d191652c31942a91fa3f9cbf42a29f8796e29d00ab50f9968fc2e347dab2b2f74c2f21134d5a651eef81b8978a3" },
                { "th", "0d5b1173dab483004cd1ed25dfd7d9e6f0103d606fb19d20ecd0d5d626a6a09c789b7dc6badd99c505ec9c28259383bb1f28f7501d26c556f4f7e98e22de03dd" },
                { "tr", "5b2639dd3a6fb7b51b257e849e757eeedb4b083dca859e9f655c610b0502dffdb7a016f2feb21c0ceb5fba889424084b1c0a8c29acf7d9f71ee00e8d95ab05f1" },
                { "uk", "b53bf7ad73a5596bdb03835db36c31f5b7b4eb53b1f860f471c19b2bbd7887a4fedca0c436cd069012a05212d4d5e8d69e1f2e6188e4dbc62a0a5a9353e93de7" },
                { "uz", "2d5dc0e96b6ee1a6f88b78b14c8feb9125d985f39e28770ead0287c75e26c079f837c13192c57340fc70f115ae796f9825754c8972238c7e98f96819ceaa7772" },
                { "vi", "c09855ec1a311519c6f9d766627c284646663682200d7b8cfae5c31555eee463cfdc364f9a459df6fb69bca9674a71303295b793e86445bfde058654a03b2bc2" },
                { "zh-CN", "387d5b8d18cb646242e6cc26245eef18d054d8f61e8febc7a30ba71474e88f3ac5c78700ee07883d2b3d3cbe47a4efc909efb2e305f39d6645a5819be8e9b5b1" },
                { "zh-TW", "d248ff66ed6fd345ff82dad53aa8e7c8934dbf17506dde96a843498be644727e40a8c020988ce0c7313c0ef68d98086b6bd0422675c6743d444f219a67c27d56" }
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
            const string version = "115.10.1";
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
