﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/112.0.1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "d810a0be657a59d9dcb0e8484c5b22e935e1ef588d8d36bad771ce02c7b7bfb719ef51d13b9ab7d3b9073cf06761daf6b888697103508309aaa5da28bacc6ff3" },
                { "af", "7bcc37eecdb78a5841629ec997a2b5b3cf6fb268e89d1437657047aea221ee38504be5897b1e58e1a21504c5700219663a12b41e16f64c16f9ba0d7a5aa40fbc" },
                { "an", "54b97e618119168e27da47a6a0a7bfb7e6c23865a380e1c666abd48b766e0f2922a68c9b354564a6c50b1c7f9c82133d55c98563827409a9cdc002277cb5db29" },
                { "ar", "a92ee514ab134bfb74b30e59db3eda09ea62f6a58ea5da10597830ef21f756680c44452693a152576fcd2ad10e3c8765d535ff1be0f1043cd2bc4ff4cff44b0e" },
                { "ast", "40559e8ad4f598cfb4ad55265a5fe43edea97f242f07cbec8c9170f74d73e73d7a093fb72391c4bd0baa2641565bd25505071236f1d3965802d0745ae8172f95" },
                { "az", "c38dcca0b4188efa29f23d4ca855a8e2036328bcb95119126da7400aa34a7ac2f60c430aa21535740d162174838966acbb53e3e12b44d276ad5f939153c400e5" },
                { "be", "ae851dae957821b5a06ada65cae23192a38baded3bfd470594964a4b6a85e87ee876d3ceccf77e927e4e591939210f9e8d271ab60a645277a30a68982d4b4d77" },
                { "bg", "2c6fe33742c79757aead343683d3e093ecb48d85a59c19ba4ef7e4a67b8bc107be901f7f01d7240b71af672e44b92b70a016f4456e61a46477b0a200771c5f99" },
                { "bn", "ef6cc023721d553659b69276d704a3047fb5d9944db6f6e64fb74bdcce943af63ae776941dffe40017866bcbb6c5b197c95eb7b1925f05c0bed09d052f6e1c9f" },
                { "br", "ec97bb1bcaf86882e48554565feaf917717f91f5a84dc4d541a6f2455b9dc51228162cb58ed549b04de5e1d5a0b9bc0a2f9a0d28108cb27a058ea02a323c275e" },
                { "bs", "0fe03c622459283aa3fd6fb94ac38bd4a1c17f2cdd7fa985d61457f650a4183bfb8e526b3aec7860496c9590b99e5c5e06cce04488c66e0fc4f65c7933134d6c" },
                { "ca", "aa4b17df57ac5be3967b828da51ce67d181d52987ce9e6a2aef9eb8f487f54a83d75aa19936d9b9dac3e031970b4fb54a57e49b44fb160b9d5c84420bf54013f" },
                { "cak", "b2e9dda6a4f803f1a74d6ac832483116dba132bf1651251246fe609c821e2281a0c30bdda0e8f227a80fd26911a157fb02e0478f0236b5dbe1541e60e9c78b62" },
                { "cs", "5ed06f34bfc107901550773d731599f22f9101433b2e322cbbfe012f4499b9779665406e8d8dfb53b596a8325cd9838679bb91756e5ba4a7e877d3fc4fa7e667" },
                { "cy", "dd3b8da5199b2d39a7b8e9b5e4fa9eba6d57f542db20ff596d3c1709f383b6d52690576da3c4be11a72f7a0e0be8b994d70341bdbcdc9efa38a10fc6dd1350e9" },
                { "da", "02f52f791e002cebcbab1fb11336154f789657838a4715e598e3aeda1246058e97c1286a5f83a06d948616ef04015e4103b7d6e2a24ccd84f51605109d917575" },
                { "de", "b2476ec03ecd73795978dfa25b9b99a1e0194ae6114aaa7e656e43e0d3112d0c41cf0a7e1853d0bb2cc91d98c4058b126a0ce9c2882689b9037c426ef314be33" },
                { "dsb", "04dc0f5fd52e5d0193999227f002a486838f7d83b471ba1e09e57f8b3fa48828157d7e45ff1a60eabcac3e397b324f7784271a3ab4170d41cdb27fb15d3d0da2" },
                { "el", "30cb5e84d9868d380835a4c9480d3afff1657eddedbd2cc143d3dca3913cdc3fd11a4cdd391e0ad7c6e50adfd2fea595201736b23019ccc464941cfbd13e8cca" },
                { "en-CA", "fd5f86e95da0185f20511a38465c5f3423bdfb12023a4d9da821a79150992411f843404db51fdc12914e923d44a53a75cce601f1d72d9e38bf55f7fc91d2fbce" },
                { "en-GB", "82039dd1c07bf619ce62a5b43713e24050822be07d768ab5ac4936b38acaacf932bbdf317d63d97bff8b2ea5ca463b2b20ebdb589d926b98f7114c70b69d98b4" },
                { "en-US", "e186e5b17493ff71af3b8d72342df295667643ef7b5db4d79a7004c78f6cd37bb263488cde0cd0c55b74f2264dcaee94d09679e7358ebe96731b112ad831e516" },
                { "eo", "61dfe8c3ae3a55633e638b256ab1e17bd29a27e60a9b1a930c8cb0eb52c3948ea2ed2102051e4c08bfc76bd76c04bd08c34c037a73cbb9c66d9b5bdc4c155045" },
                { "es-AR", "2b5ce0e4a539de7a96936b8fb5c3b39be0496635dc8aa7065e55c0102d180ebf7f71b91dd982a065e2a5a060f88cddc0bde498d9aab12fad4cc72e41f3ebb469" },
                { "es-CL", "7c813dadc5a223ce2b856e7f9f863e6692711c56b2f22dd7d3ab14e680ed9654aa56e04b24e4c21d2b5f1058b9c6be149b5c10bb304ebc46df565eeb36d33373" },
                { "es-ES", "03b837d5239506221546c8d1bf877623aefe59bbb9445f35ca7e0e24f17df483a7f7ba9f706b06e748a68ae34ca08418e9322d143d61a499cedf5b0f5bf4c776" },
                { "es-MX", "2b168dc6ab73807c075e47fa589d9b040997e873218b457affab57c77e78b2095618ff6ffd80a2e97360c080bbceb6615e1fe3766fab0867f35c8733c09d0897" },
                { "et", "32dd381459371f388fe1b3441081d21f727e0731e7fd8c82302f4b0227658b51a9bb90d952bf5f624440efface9915a38b7c3b9ab53e217b62f547aa4c916024" },
                { "eu", "7391ef8505e190c9b11ce2dc80d860ea52789a64aa367dc83de0e65b714fe7c2d28934ad80a4bc6c5a4b6b8be300ac15e065124824e991bae14049c7db9814a2" },
                { "fa", "0f7a346fb5f24ba6560632b8e440488800fe2e10d900ef6c5f2b56ea5ff567a4fc02a85275b67d89766e52915fd637175e6a466ed51807d79856a69cac7e6b23" },
                { "ff", "f40a20874ac3a90041fec4f7a81722320e17fa125911135993c274815481373501ed80d9d25f14c78827b9b664414005468c72886623ebdb3234436fbf827a69" },
                { "fi", "87e03f02bad86463bc184cd354bae3c6966848336d2de35942ec0a690d670309acfacacf1d553c5b43b92c7fd43ae51e2227f6d394479bd939efe8aa01409eca" },
                { "fr", "ccf12cddb6be630bec66e43855a0d889fdc57991127d9e65fe00c36d4c485686e2925d34f40d126eb41da9ce595fb32991a0d584cd84ffe7a70bc43aab866ceb" },
                { "fur", "d9a938212fd72b1b1285591a6a04de259ee5530aa27be272c2eefea875e5fe5a312f133b92af0010e42957bd4fec8fe39153c53a34d99358a5db2d21fb577488" },
                { "fy-NL", "7d9f3e224a3122eb5fd179ffd50b809a0d5530361abd165db0862c21868cff16e9d2befb4310a2a3152757b5d9e954206df7df4a8e746810e93693cecf04bdd0" },
                { "ga-IE", "3f036dd5520cf97215bf5da7455b84931ae8b41d4d73dec5d51376e2f4601e18f2a5d500daf6d5cbc784d7e974d54b08fe78292d43346f5b324e99ec506a0f75" },
                { "gd", "37ad1e8fe19178d429b5d723b73d76fd5205c4b41cde80da56174d487e8cb868f8d019715453a4bbeb5fa35ad071b779225d770cf45bd843a6cf237d0a7332bb" },
                { "gl", "ad680e47eb36487f556efeaad3d8a00fccbbd682f660cec3c899ec49eb5b212a564a297aaa3f35ed61d497b80548a6d554b307fac1a33d90f60738b42dde4394" },
                { "gn", "09fa25bc6530d45632efd0f4e1b31a9b93190718955319d3cdcc395cae7538037894f860ce4431fe5c952673d55f462de0a735752d4acc96b4a12466bbbdca8c" },
                { "gu-IN", "a50fa13dffc278c532053f550764e7e54f34ab1bd727bfe0cbd361576e669c6941fe54ff0dea70020d99c9dfc73d3aa84a4ca09c48d615488af9dd6189408578" },
                { "he", "8467106d04a7c5406c793471459a7aa266c9fa728e499e3133881c113c3d86d3ecaeb3552e5c90b0c1ab3b7bf2dde68600705b95167b77aa84ec9366d8271775" },
                { "hi-IN", "1296e3f38fcab5b167939127a6605ec9952a20b647ff83bf994a08fc54f21c7ac5ba2056f15640e4db2730c190f5a6d653f80701ce47235ff839ef57d11c2060" },
                { "hr", "d88c08c1dc53799327a462743362c574e81ffc0da9684278de4fe16a00b1505191fa60eaa31279ce10d19854b90d9a4b49a5a2de485c2465e3b1fe5b37daa198" },
                { "hsb", "f440bc5846400b1d18ca554643bc6c1d1c8b9a80f329cecb490f5afc6063034ac501aca98231aa316267feb01a45a3de895c168ed0b4b89ed9eb6ac4bddc47a3" },
                { "hu", "152d5eb6940cfca77e9cb9c5eaf3958a8b8f360d0e7872e70b812070be400c90ebfbe8f0dfde34c9e791e0a5d9ad0266aa51d67515b0bbcce6ab087fb4a56775" },
                { "hy-AM", "36d7ec5c12ead3c39e26c5f658fc23e95e1907e6df2a82a55bc909d25c45a2cdcc06dc7eabca84dc8951f2fd448dceb2fd9df706a5694f2d5be790cd24813a7f" },
                { "ia", "01912c031f6432e8c915c738ebb3943207e5858752f903c5a31d6179717265567522923fca45fb6fbca838ce2a3de8ee9f71765d673fe39ba419f99da5f52c45" },
                { "id", "d9bc2c8d275185b89a90c25eca18454f9d89c1652d9851855c9b243bd7898a0f1285f24b2646f720cb1d0f7af63f617745fafd720165a5321546a56d65d772e1" },
                { "is", "bcd40d6274fe706b7d84b8d5879d83f450658016368cc2bdc3a1d3efdd0d612333c2879080922ae2674bb7fae8d32b562294c06a3c22cde517635427761ad06d" },
                { "it", "9baff682cba2df3ea81ec2eb8a012bd5ac46de9fcfe46cdbb4f056865e376b57c5d1d3b8aedc68f3eb3f371e3daf9374b12ab8413466d8227f33c7b7c5e70904" },
                { "ja", "f2f13ba45609e16c9ce323b3716ec9b7615904e484fdef65b595e152be9f7284cce46442e16d84081d10d767684972afe63a0836a3f7f29bb5548badfdef6b87" },
                { "ka", "e72b2ddb05632cd2f08fba4737cb0ebccccf66f5f988eb8ab066ba88305400767932564680d77466b931164d790c49671451fd2a422e3f9381c2f8b795786116" },
                { "kab", "f0b91b88d18123f34c7108765e1f54f7b1f9d16b3558f281ea49866840b0bdf34cbb5ffd6002bc89a11b2152c17e78fe8afb415d98e756a12659e24814c1cee4" },
                { "kk", "b50f38a874962d2752109c395396512099b6175562d653522fb30e9871c9efc8dbe91b35f51cbfdbe6d190d258728843f44d315803c7100547f9e48a6019d8cc" },
                { "km", "3f18b7e1fe3b41d2bbe3afee4808aa419192c90107792a154a8f3dd04f8a06764780d7d1b62f292a8518b86790fedadb2f5bcff41efafa45eeaedc1052bdb688" },
                { "kn", "2fa4c8b955b183b34ba056dae69ccbb52ae1f2c14b0ae38614969679b4490b6a8b51a201d9da797687c612ea36f7af203d1bb1aea95df9307e474647853da8b4" },
                { "ko", "04c894cc97f26accef92b7ecf080b12c74ce07aa29824f014f918deba19707415ccaabd264dc94146663efa99aa6dd8abbd25ff4551ce764870d8710ad7f0729" },
                { "lij", "2dacc7a10d0e1d0f5227cacb55ccc742c448b04e0dfa82957eae839e41032d158a374084e3559c2ebfc0c0b50a69e4f7a0f56d97d3679a1ae3bb44b82e1f161a" },
                { "lt", "d17aca1c24c5291893661eae680b7a32a5b38bf9a2c86a7b37cc4865a4b4d02863d489eddd1f452e0c41a3bc38364013b7e096517d360eff151c76dd95cb558b" },
                { "lv", "0d347afac557af135aab25a5892f21ca6cae49b10a4469ae6a94b15f63af2a91f5f4409df221436207110eb3e0f61669b335a286958c80817b7ce3be36216553" },
                { "mk", "9abd08a710e448d4bfafc2262d758b2b38b5bc7b6ad0d6963cee4820d9f81d4c8aa5ca7053f4522524171c46ce0cbd6139ae07cb2a41ec77050334b3e95f2c31" },
                { "mr", "343428d44bd1ba603d5b4cbdbcfe68876ceb4b7326a6d2bb215c3fa11bce65db87c7c50390fa66edff10e5893c1bc207c1b607e2da1d10fb3a1289b8c346edd4" },
                { "ms", "302509d2f123d9cf5f4917b664b8fcd93477294e25705df61c9105182db25430360f0a3593717a292964a97ebc5b6f071c2fb0e526dc7f06360a7533c97d9608" },
                { "my", "9641e83e916f27ea47c07cc82dd90b91f0c2cf185ccf845d9da6404e4b8f0b2f0837ab51ce0a900d0bb0121ff0fea1a6853fd7923853d102fdc2879235c219b1" },
                { "nb-NO", "ac494a1f69cd98becb817179defe6cfb7f20dc847055152fbea380b2382042eadeb2f377326c517fe534db86a33a1f0769a6538168752b8cbc748290d2765f77" },
                { "ne-NP", "e734ba064697ce854db78621841f6dfdcb7b636091b476a69251982ec66f93be0cc20be415f9d6dcec2c2206e4dce8a97a7cfad15e33c1ec1d473283cc9bf9cd" },
                { "nl", "abbdb8c98b97bcd45054f610745cf9f03d44f73449235b255f80c298b9ade19d32589f819638203686e84bf37d109203ccf0070626768b30f9115f37c7cd5452" },
                { "nn-NO", "17a435a963645d027945f7d18314cc09f1f9ca81d5da05966a7ff5d32a964101e50b9af327aac39f0ca7beff629c6d120d887a7a977bf27a24f79847d823f9b1" },
                { "oc", "bc6a03a935b8bd7adfdf7a9e24f3e185bc9022f5acc881667a606b17685eb7537c26bb04d848798247edc9ecd8f1fd343c1c9c09d2a3c66da0570f9a3fe6aa91" },
                { "pa-IN", "2ddbe785f20ad48d2b2cefe457a618941a832630241ba36d3489f688a3a0a26732c96045b2abe42f13f63aafb8a30e9974e166f81afc57e10b34fd3d4c2d3ead" },
                { "pl", "6463ea42ba6b6039ceee4b857d469b765646d025931a56f25076fd43e6a81ed93799c29eb0455b37c27e78202ea3d036109e7093bdedb61074c87ec517512aa5" },
                { "pt-BR", "ce0692a3ed04028ac3e000f939d77662f98be8053b3f15bcf1f2decd36b4b39a3b0b96bd552ed0e9071b03f0f6ca0f64d050ab020b161700e73d3e093b2422ff" },
                { "pt-PT", "2e3a2a004d6226739c889e48053cb9cb602a49db2e694d4c09ef79113f284d2da5013b95b98a5561247827636dc38ea815a646640098e0e8aa6d50156c36cec5" },
                { "rm", "54b4c87a6f8bc290fb394f6d4217dde1c2f2e667d7b32c6f7bb89501bd801d65eaa8e65e6d33267dfbc1021386b518204fa7f7c30b9592b2c9641bd78f9d87ef" },
                { "ro", "dc6b9cd40592741ab11af9f331d6a3ebf01f528eb86803840acfe9106225c5d321ee980d899c0828223a6cfeba43111494e0355ceefa994668c66f7d6d1ac441" },
                { "ru", "a0e099085b5279023a17c6e27b9fb141ba3dc3ebf09ae9e53d4efb74f1b205ce40cdbd8e01af3e371ff955362c0f483b5da3265be4e93d88fc8646a6ddb35228" },
                { "sc", "023ef8cd661056ac7ad74c99de7d8ca18168f1ec1fa6270f58c373fd6babd91673847bba4d2e922135114667e470ab6f03878469137f3571ed3a72442250f03c" },
                { "sco", "438bc0e72a45d751d5afaa863d8f885a419aa461f0ac902f6b6354eb04f3e7cdf608bbb60990ba171d236252a8eec34f5b3f030735d3e30d519da5639977dbf7" },
                { "si", "1808ed7d0b4df12c43f79ccc1e56061e9e3f18a57438fa80474d6e6e8ada4febb0dbad69ed169c678583e1d4efa7bfdfdaa4a04badcfc2eb489add8dc86148b0" },
                { "sk", "0863c28f4fde43bf0a979c171122495b935c9450aa95c8cfc6ecc7f83e2211c4e30eb8eed511cac1c07cd5e2f24cc3030a616f5994ba8b161234a212b7a045b2" },
                { "sl", "58535d5e07194015e235410569a13c840a1b12099d8825010d90632dec3facf373b3db8adb98581069034a63bed5bfc881223dc79d462f79d7ea40549c8f46d2" },
                { "son", "9c314764b5561bdbfd18818c08d0c17d64dbf426d7cf1e14cfbe2d0fe92090b98e7363ee7a790c10224f569c2d6d0bc514a75f4f5f86af69b111bcc74d7802df" },
                { "sq", "0b350048962605a79fb36980906100e5598bf47424bfb8e8e906662a8ddaad67de92371184b7dbe4a5890c6a86e6b61a91844a4e81f65646d0498ea65e36b501" },
                { "sr", "849f9f08553bb2794f00f086d4f0947dd2c4513894e0bfa02054fe73678297fdf82b918ffa2d57bb77837137cab3d2fcb7144cb22220d18741f3b33151c874ef" },
                { "sv-SE", "1fdaa42e94ca96a14e3bcbadf0ced02e2e0dce9eb016258f7fea65752378f9744bfef7495a3d1e7bae0e9d27f139bc60ef22f11175c2a182f40f5571697821ac" },
                { "szl", "9fc0733d3e4f06a1be79187c865b29c15628dcdd7ece54613d8d0a5171d00ee605e8dfe6bf32214b1df8fa3daf420055674de369b6822a61a295c8143574666d" },
                { "ta", "d226688dab4b214f6cbf6818eaa172579032794603b2164cd3a3598a553c94faa21cf682caf099efa8c8ced1dacd84de5b05e9233fce5b7168ffe728c404c606" },
                { "te", "85277840d0b3900bcdd1cba29ce9cf1c49644902fc5e1f2ccd5c10d20750ef8211526b828f14a6834898a30ac85294f1237fdedc96b342e3d837e6674668cc51" },
                { "th", "c4b363cac70c6e695a45e9885c21f90b69b76576a304e87ecf196c26e7d1d3a76c3bbc107e66bfa7005c01eed73e30c88f53585454f388d6d1be6a5ec46f1c95" },
                { "tl", "55f2f6d227bddfbf952bbcb8f2560388f7f00f968a03d50d115a724cbd5be23598d0fde5621995be91c32fc2e8c725adfccbc972bea316bc0a7f2247905cb1bd" },
                { "tr", "1cb7b6bfd999bdfa897940898a5d115f334870705503ca3d649372e2b9bc68e86846f33b3251df22160d5741cce31b87f183d225a9ed18e447b12ef9aff9bd9a" },
                { "trs", "2be29ff43fdbc820a55a6b26861edc555a3436bac384b68388114e2eb4eb32f3532973dd8b752660401c0bccebefbb5cfd04f3f84aaad81aec91ae332d53efac" },
                { "uk", "5b75f2ef9f620e62d18cad7e15fa897158f79dc40008d5a51bd03f3d2687704d0150da8f553531949719f100f6378e12bcc30ea17ccd6bd6ba0f1811cf45ca5c" },
                { "ur", "2ac92e1e097276ca1236102b7142a541aee73ad43e7820bcab0aa2f8f082ea20c99ee103ffe0b735aa6816d4607ecefc7957e935a47cf83ed6ee24549a494b82" },
                { "uz", "12e313b769bb4f08d6b6ae5cf539caceaced3f406325abb7c28fb6fc108cd12f11e571aef737bee7740823b92b50edb645a2d571b302b9790d67a5235a2720f1" },
                { "vi", "420233c4f8b42fa699581363457ddf8ff1089957e0756d90f4f6a23472fe688fc008c9cdfd79d7490d87fb647e3a5e2c0c6b9a3c1e9add0930193a0312599266" },
                { "xh", "7c5f62148c8ddab996538170b104ca0616130a602ce5a52cd9ed1bdb9e597b8d52eaf1e3725fc21f6127dc40dce20690be472ebd8fb82bde38cb5300d195604b" },
                { "zh-CN", "37a2512ccbbe019e02b035cd45e2ca8680420ce456294bde6cde58c6a50e60be8df4008bfc89d3cebd7b3327fb919ab4f1eebf2a17adfac5a070c1d24c03be33" },
                { "zh-TW", "472f632859c716a1182e464d99fd41c509a3dcc448add40fdbdb8a6f30885aad6be60bfa70c614123ba173f5f7b0e60470efb29010f7339855f9411dfbc09181" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/112.0.1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "830ff23c1e7c248583c36dd3164c1e23e5981308de98e828a2bab7c84d76268f6b0dc8bc98dbd1041370ece02c546be7a763f887149b9c156f8ac9cd9c6fe91d" },
                { "af", "bcef0f9599d02d415111976e34144478e3dfae72a8e119d5af735e7fda264b09eb92ab55d486f6c68216aa6ea9f819ebaf6afc4d02501c01da95b3c98bbd7388" },
                { "an", "06eda4b86fdea7ed7bc05143c58de44e6ff9e969df4c6972da94dd8bc6bcc6d10e8dc61d97fcb2c27c655350826b2e2436535baeedc7dbc6e879837b3d488548" },
                { "ar", "abd446bb49ea1f115fde6fdfb34f972fd1a2bc07b23b43bbe8ddbc4b757e687543d908b4d668fda1bb5238d9ea72e9f6d164be7ad18c1916978ea83eaa02ef2b" },
                { "ast", "397b3bf37a73d14026f2d297706f70e662f66bb29c49436c313d6516f454c36cb9112b4a69aceea8f67f4160d0ca4e70965a9ae9353fdde9225950bbb83ac325" },
                { "az", "4b91549fa4656caf372583d9844e5d4f54256027f1caf422ec87e0d990d1846468e9a324b72729d9fbcaf5f82e3e14b1d772f5d4f4b672471c75b2d84f8b8270" },
                { "be", "99049429784848600d5e4e08a9a428623ea4bf43b23233c0859108eed4881609f45994fbc0fde11a7fbc4275cc63bd2439e5ea0e8fa6d6d0f08425f2ab9a7156" },
                { "bg", "c201fab2e044f2b1728dda5cf622bebed7039ff04f716170d08b765c034d29f6c8983ec98865ed5185a7457b5945a60e1922d48dcbad83001ab3781601646854" },
                { "bn", "49ebb0fb3265a20f5af434ce7fdc0152d3c8146d9c952edca0bdee746f9d5ce8c6ffa215702938bca0d7632547a5fc8dfbabe106276e1dcf872b2cef931c34ad" },
                { "br", "8c7110f5f816be98f5e88c8fdd4a66abfa248c27db2414364df504299fb234e86c53c1e53d4fbf1d0f96801a2d0fe2d20d2371c6d8d6a308c4d1b0cfda47dff3" },
                { "bs", "f796d2d7ffa9f70efedac35266927cc0632e7fa51815ce7e4662030ab870c5692a11058f905181fdedeab05909ff83d5f752e907e537e9a86152c5e3c78a90ea" },
                { "ca", "c4fc4116beaedebca7c174226763fa7846c36a50dcd2de2d5401d91378b4b13814cd18f160bcfae759b1941a4ba7666c745c523aecad91c3bc1ccdd9bc61f18b" },
                { "cak", "19855c9298cb7ae5d6ce13dc86d5c7deee05add57c448754362956a3d26769be02348c5a25e749915fefb8c098b49384e88cbcb2fb0596efa2e5f6a10c95714d" },
                { "cs", "af566b42959caa08cda461d05cec1f2365910d4d07c1a05c4787f9a6f539042d661b3bc85058b3c1c27a829d1ba4bec0eb1dfb572236bea789179c7a09074d26" },
                { "cy", "5f098cc8057fb5f846b0cdd945e084e5e6473c7a4209c579b0b2daf8240cf21bb9da53f959f54db06b66f4e9e43b720860b71e7016e3933f2fe1f2913ea448fe" },
                { "da", "9c13d83080f7c37a6c9ddd1345f3986af1702a8ec1c2928c8301c1bcef928c0266ef95eb928c4a74cdd52e526bfd33a00d7f0db79dc74828b1c374375e7c60aa" },
                { "de", "afb7071612755517614d01ab07c767e514050dd8a612a2ddbe4e88b8dcb495f89413157fcf7ccb17913be519ac65c5d2d49c22cc35fd3e5e00c24b068e372503" },
                { "dsb", "4b0ca31bddc636b72b2a1fa40402b0fab3d1572aaecb51baadeeda91acda6c30cb29c55f8460cb7c7a923642a4b05266734fbe380c5c54343634a1e189aecdac" },
                { "el", "2c4a0c749653f47d9f2a15bea0d264bc0f1cd6fb8dba36b7e826d0b02163c13fce3270ac0888373d6669fb9fe8e1f221932fc0f72cf32b6257bb83587209b354" },
                { "en-CA", "60fc2ec99c8219a1365c914fc6d40d263bb15ba485b5a7120a4a7acb4ffaa3fb10143bd96db3be56a0702ee85546151849fa6b67c88dfbff808de142f252e94e" },
                { "en-GB", "a02a12853da0a3006d9f9cb7bad78435723e1bb11c8b8f95a69e5393e7a8a276808fa93b6d7bdaa06c5618c942cd269172b46947170b714386db77d8787d7069" },
                { "en-US", "0a6b6a16fd5afa583ce369150ee50c23e6317624614a5beb8e8fbfd6ad3efbfc5b3e35fd104964e4f74e4dcac5dd9b90af4936d95bcd0b3427090e5490f776a4" },
                { "eo", "be2e601adcab80fdab478581949e0f92157aa4ac34a10d34ac2fd9c80fa8ffedb7024b19969d8f728969ad1d82673b2acf9008f47d2ffb84e2f8e5c71d53f84c" },
                { "es-AR", "4efb0842219cfcc5ba3bfdd2688b73d891b3f4560ba2931b21d5b05b1f5b31fb49c4fe3fa277a8d24b104b3bf19a0a691249c9c0da9c13c2f79398a04b22cbe5" },
                { "es-CL", "8413fbbe6991c4a0ce1c2b39ba5ef6f26da9a2473b3254e8069bacfdb6f4c139bb22cbd91db203aa3b3a197614a0008185b63a25e51eefe88e0483995b5444b6" },
                { "es-ES", "b0a4b14cbff78e56d484f6d01b8e5084c65164438a95958b15927f4c21ddf4fe70ad0680c62d555fa1f2f0ca61dbaeaac56be42a8d617fd3977e9879b189e6b2" },
                { "es-MX", "d61b8f2555c43f2ce2f8a9a9f58f5d0deea0a478256665e6e1c73e8fb593e147b6452b699444823cffc358cf7e701fe8213e01e925ac3774565099dae02b0967" },
                { "et", "18f390ed4ae9865af86087a9aa74a7edaa7ac5e98c4b1d980affda257fa3315e1d5be4cb347d8991c8a1640e8ccd0f19cc1cf3aa388f6a18047eaa7a4de1d91f" },
                { "eu", "017e1b39ab9f8de6dd69b11330980db11760eeab8243ab5068e6b5822afc969db81932130a5842ab924e3382020a3ffac0a2076527f7bf38ccb5774e887d440a" },
                { "fa", "a8d19505d51609697e78de6f86eb3a2dabcbb0cbf72e273d30fc615a256c0202be120d986a9f5a0ddfb3bbea75da1485e867662657334fab355d1c7d17bdaa37" },
                { "ff", "80b7a90ed7ab33738be893928ce2fc26cff2ef0bb8a031640de494a1ff74a4984b44293fc80e06f2da13f5fc13b08ec5baba3c67c872ba6ab5dc5006bf42b8cb" },
                { "fi", "497209892fe41f2bf9589c62644e59b2a6fc59cf2031d129092c1404a9c14240c6adf367c2540b328d9e6ad768087259cf0125ef99f2a748569d85b72459dbd0" },
                { "fr", "2fd13b370d9244a118599b818c71f6b2ff120689db2d07f7a1f0934f3fd223c38a3380623b2890bc57b2c3efc6745202b4d2b416885b34ff45ebd448190d7ade" },
                { "fur", "89c6c59ac265bc38a2bcbcb43f3f0ec9d7894d2780f7aeda77efa7c061ed9d26cfa9af07c0b71e1195cb60e0e2de7660f599e7d3925d8bd331c9411cd409c05d" },
                { "fy-NL", "102bb47bb4cb09efd3eb77ac76927d6f74054543b173a95e58f6b719184721c1612a6d716a19d7b86ef95d42cf0aa6065177cb87f720ba5df152d4f96c6343a5" },
                { "ga-IE", "6c124f4cc2fcaadcc2ff430377adef893a61b98896028a8d4fd5b1588944e8ccaa27acb8f82819801a2b6124a1c0a5efa0b73a5dd9ae6089f3484d8bd66c36f8" },
                { "gd", "074710b9f96bd7b74e7c05bbffae6280f29b0a03cbc0aeed19936200afeea4328a0eb85733ce333ace149a3599fea0f6391ec73af4829b0d16ddd5e06b04f410" },
                { "gl", "3abb736a456c0507887191e4f5f74f3b848aa1902a30c8fa747754a58f743165dc757b1ed770516b9bc7a9624c5e6841c1b6c9c62a415273480990ef4f9e0981" },
                { "gn", "905f44dc86b0a45b4e0a326ba0baa3f508cf8ebc6a7339f2fb8267a19c81fee69ad1b771e48585ac1841db6a9870b0b0e2286a9408b2cbe8a991becef4e3c627" },
                { "gu-IN", "12358047080d8c9c1a8deaa4c6729caaa8197eec42083627c9a9caaf01342f4b150d1fb904109d74bc76570c700613cd3d690ad66ea1150a39dff0d61aa2792e" },
                { "he", "6cfef81daabca6027490fc2af3cf4341799053ec3e7a25b9b60ddbe5d9f46937bd003feeb56863bd52e2fda24d87b9707ee0cf2915edc57bc465e23d095deec9" },
                { "hi-IN", "5f601ec13228690a45aae14c1445b337984fd3bb9adea8b21f1fcd98f0abc8bec327a7f1c26fd0f80874963bed6393432be308b32fb54e8e5e79ae410822c88e" },
                { "hr", "7a7e1a3125a066f51028f8f16db5e56f37fa240923942e4558b713d9c4a00b48094bbe7763d292059c6b08c2ed98f74ff8b3a5338da79bf7217714bb88940e95" },
                { "hsb", "02d588274c8201b922f1cf2707ec0f086695f31a345774b6b9dd04e15380de2d0296d15d27d1323e4b82d652ae1801459cf93f342954d27db402ef1fb84b170a" },
                { "hu", "ef94d5bb3246bb810dbaa1c8d05d97433e476f9b5a4e807e50062228124d95da7ae44868e07630b8b0b4b9560b55b73d91039d822a0ad76071d443b1db01ea91" },
                { "hy-AM", "37157fd7054bd6f8f527d4dacdf27f60d05bbd1c48a0436c404e6259c05a72c22cadd9218bab974681716832838b569883d7ddbb318d7aa275d366c38104e1b8" },
                { "ia", "75d7639224e0a1e76f194be28134dc2db53e5b9c1a5ab7d2a50c16e70d9564097dcda4ea40f88089cef37ac61b0867545b598b7cb6458be4e665a78c4f677ae0" },
                { "id", "bc5da929abe24bc539f8b157a2678c6c413b140bff6bcf77226f5fc36841eaa2b063a0380845de45ad8b648fada146770d1fa79fa7bad191f4e16b29d91c03c7" },
                { "is", "42967b723c500f17cb99bdfd8e47cf38f76dbf45003685b619b14267c8907f980ebc8594899831e9c5bbb543d3e4ab0f7593719d1a86711fe15d7adef679a12d" },
                { "it", "1bcc010feb3750831a55157571aae9f3ca40203b21a5c6b382a281f5bae1d55f7f70dba564583f7ea54039e3d68b46649e666b3dcc0d769dbeca18607cc230a7" },
                { "ja", "d65a9b90f1c4e5a53ee87adc6b6bb31d7abedcf9431b8253171b89da2f892bd04a1a3624fbeb0989ae97d51f70a3d9be49fff616ff87ffbdaf1b960e749ec86a" },
                { "ka", "72c7c2d71927feaf1084f34ffce26f46cb00cae35e1c8d092cebebcc954294850bfab59ea262087429bc4c38ccff0b6df11d1d0fcc402677bac9b826e706330d" },
                { "kab", "53a56ccbb33e9bcb436823c01d511cb473e022cb6ede655d52dbd5c80464c16e4f781870c61db49a696e025ff1dedc64a0e5892d2327dcbfddcea347cb5ab37b" },
                { "kk", "1a354d326b3a8793639a19edc121f651d404776f54c8952f4c8b535a5e46bcecf32ab59dce4dca2bb34c4589f130d9906526c5f387c7ac74bd22dbdfc641217a" },
                { "km", "3398f174184ed198f07c4ccab3ac402ef652a85e11d6de56037115f76a8df976d14625aeff36e32a80cccd289cd1fdb2abc4493ca4acd640859c34fe22cefe59" },
                { "kn", "5d4977fb0579069cbdf0fe0bf81ddb13b02257599181905797bb1d063eeb8ff8174c2c8cf0041587e3a0f9099fb7eeecf7d29d4c3d0562cc780d9558578a5f6c" },
                { "ko", "768d4a3dff0426c5249d7da8bdd98070dfaa0726e30c7b021f332d2b0919db4c48310cfe1d5eb4de28218746941d8e06ae39a836d7fbc1d94aa729ea1d88817c" },
                { "lij", "c81c65349b9f93c857af862a67f694ab5c82030ab19443a8690e9764a313d849ceb78482ad5acf2ba8a8709d3cb9281397444cd2dc65ca9ada8b50b250dba386" },
                { "lt", "63689e41f5b124a61dc44353c04f12acbd7ac244425d4ee54b7719e6bf1e3487cddabc4f009250475c702b58aaa50238833017599dabcc9d7c867cf4c8d942f0" },
                { "lv", "f8f4036158b43daacaea9bd9ff2833968da43b4a3b882b9f072afb157bed4c0dbc18d8b61dc410652ee9cfe26598627914442bbcc04953350bbd16b417b9d37c" },
                { "mk", "1b35bedde16affb9a38dcf10503e34eb6b263fa115c1fa1362fdf173bd70c687c4c443f680da0a1b6b01382e7f633c6c86fa70c2e22af39e69b7ce92ac794aa8" },
                { "mr", "65f32fde4f1f0975ff4bd95458d577edc162c8bb104252698b6ebad8c910798b2ae26d054b90549f7b992d1eb8ce82df2338c03daa0e8e07eb6f4dd3630f5887" },
                { "ms", "0b6f0eb470af32c22e133be0f4bc7d6af4348079a3c191c3037063058904978f71fd670533c8ac7122b063948231efab31a9f44097e8cecbdeadbb04e16a4b92" },
                { "my", "39cd748cf7b5a7960db47eaf211d854fa08fa84897f11ed7e585ab40d4b659c70d871dce554aca2c35884cd2f68f813f304fd68193bae2d07c195d89b2255acc" },
                { "nb-NO", "46cd8c6e08bc69547b0d8db517afca3bd9dcbda4a8ddd18f367513a21d042d881840c5abb0f22b791d5b382349085610578f7969946b9ae06b8a3a2e9a55f7a7" },
                { "ne-NP", "a0e46df651430f4cadb8388a47c3a0364f900acd5783e58af9f09a1a3fe61af70846b6b8635863785a6fcf830a79417571081a4e4ae686f57918d58ee78fdf70" },
                { "nl", "9ae8d0c68c4667ade4ea714fec34b9ccffb051550647772e722afa5592b9a6e308586ff6af61fe809d73eba878babb939a458ee8dd90587975a575ccbbf8a70f" },
                { "nn-NO", "223b617b51df01261314dc3eff335049507fa9943a8a54c9993df256b05decbc67ae2fba39ebd6a49ff9ab1472869aac01137a20e7cdae5493dcd5285625eab6" },
                { "oc", "5bd85be5edda31027b97632adeba650b078f6203bec5accab05b22568388707690421718e22c9e8889a7dea57f460b200483b8c86bfc7cd29ba83392faab4b7a" },
                { "pa-IN", "147458a65b8e49aeff3e82fdb1dae556093104d023635b2e0490171ae1793d20fee6daf414a81b5af4ae608d3d3540b86a4c6a5c7a4581dcda42aed9ffa55c03" },
                { "pl", "387347f6666a1738e11287acaf74cd56aa8b4e37dc873289b2769402e47d2125da4a59a5a49f305e4dde1b9307b902b2031ed55c37fbafa59321d6d4c860ee43" },
                { "pt-BR", "fd1050c4c9b8c8bdb751cbc0a3cd47c0c450e066eadb43e8cb1135db84b2e6f18e0c4a78b21deaabd6f52b0a7370eba9e5f5b5487cf4e39cad18350234889433" },
                { "pt-PT", "10a7bb8de8220522ec799b2738caf8c7f60201e42910dc8ddb162976c9ef8a5d09dab84f0cd5c46acf05b481436a5db9c9a67644dd25aa1896f2ebe32444ffe4" },
                { "rm", "99d7b4770b71e4f55c84e082c39520c3ffe0af116dc1a4511ff8ce939d5de4f4b23336cf4a5a31ef384ebd5cd4a6acc9bdcc27855b8ad304ae669b819d871020" },
                { "ro", "66bf060b8b1dcaba2c9e2924851aa1226618d22fac23e29499b9ee3fcc8ab37a27eaff5c640d06acf6a77e86a160b8c70fb73395b41cb3bd873027773a91cf0f" },
                { "ru", "99d151b7c653adf09fddbd91ff46208ee59d99e7cce3ce7339fa56c52143feb8a5b4e45716f3e6498d94c1cb8e3bbe2a1516e24abfe3abb1bde45f7bfba7fa72" },
                { "sc", "c1f9dbfcc77d771460a78cb63c5554d39f48a7842b45c6acdec4ceba55f3af52902c9a29bb3482f1d8efdfbe3c4fe652d220d65874f0ca784b7c3783dd10750b" },
                { "sco", "0a6fe068340ecb25ddd14bd8e22ee085e7d3f1d6345a15b46ca2bb5fe10b1b6cc86cb7c50f3dc45a3ba00476076b22addf894af2459f0b4372e7641f668ed043" },
                { "si", "932e741a255e7e12ed573327c07c22fe5233b2b100c97c30cbabe7b43d17344dbe85ab86dd0bc937c717849ce49186c19d44b4e8128042f4cf62fac01a8748ec" },
                { "sk", "9678e3d47a253139152d55479d9cd655285da52eeeaaae7f7409e670860f3bc2ae92bbaf13a6cae7378a959a210a21907131707972ee8bd74ace1350f9cf14f5" },
                { "sl", "479c0aeffaaced4d5d6097084cb2ab3a4158de9209ef7973628a21b6fca44b5714b5dd26a3851dd10aab379e8d0623c458f5469434f18c18a27f87ac8d19aba8" },
                { "son", "617f449439c92f288e45f2f8b692129c2314f4fc1bbb4bd068b73244704d4942fdf419111be652f00e042d05de6e7f68627f07df23f3d83b1649303045407349" },
                { "sq", "6767f4819b7e6f03f8533284c1b3dfcd139eb23346dda86b98f282f64ae0e5d868467ce391dd3dff194b15f19c7436d6608ac818668f1fc1acd2116cc7fb4933" },
                { "sr", "f28355ca3902a819cd74e84af3b82f81816e57aae306ae9b179cf61f0b337233dece9fd8884f77201a83e475092164c9619404ee460ab548a3be0293433cf603" },
                { "sv-SE", "21522b90762d3f9b71bf857da105197b1b5030f2a3b7bf3f864530e0ab099afd5084b1164b102a33796ce3ee998a5bc8d0d424e37b4107f28bf965bb3d89baa5" },
                { "szl", "662d679aa9e3840362a2b4e85391c363bcf4ca3e21ac1f0bd6b24b2d940a7ceea62131b4dab800c8458b31fc9572ea2518fc533addf14cb5f509bef45a6d2103" },
                { "ta", "c83017b85f990bd3ffcf575208f0ccbc650c5a3c45933e20d0c9e3cf07e9d41445bac216667c1247d7df887147e51257e05cdc7e6bdf52b609446e0a0160dd35" },
                { "te", "76c314ccf55cee4d5a69ce2886964ae88c28b11ac4f013f2e8589e687dda6eaad3737d7f403cd2f4a9754166b5f8b7383d3756ea182926e8cbc7bb91a71a3924" },
                { "th", "01edecf6faa9e2142f6ed0ec384a7aa50d9a3b312f76ae19384767865169bd235039be4ed63058761b41104ce7735d484234b7059190dc9045da9d1588aba9e8" },
                { "tl", "2d2d392041874fafe20b2db9d4e6c55d28faf5f30d064f71ec3f8783599091a1ad37f640afcddd0c03e887322db076930e59de26a717f6dd2698c81ebc6abf67" },
                { "tr", "f831f0018df6fcd7f11480ff2a823c102124e119d57019fa022642e0cf963397fcd3a6c133b0ee16956ad4ca7a971a90861993cb29de877c673d71086bfc57eb" },
                { "trs", "c51e9c7dcd29d75768de5357713c969e0020f44c075018e1d289b4ca5cad81f34f3562437e1249a751ccad29de03b1a324c5da49e48a9c1acb3f99a780d5d04a" },
                { "uk", "30ac5a149fccb28c397f2c9a5f4fa51256f9bc2ef10b54ab88135432d00990bb3a329b983a1842d7d8ef8e4cec791afee18ab4958da8786be39a3876af723932" },
                { "ur", "30b3836c42565af478f7a980defed8764c32375b714caac55e35722e85d9cb874cf4f02861301603a9000bffbc701aa3e436bac3fb899fc7e503e11dfd20d114" },
                { "uz", "1260da3023b77883f8ac760d2b8f8382dc6bea278239e363cf006ed8246b5eba053d2fddf6530712b4b94667516be4e61bab3ba559d60e039be33d7e1142fe22" },
                { "vi", "e098a8b7c99de12f2600768bc8c9b56432ba77e66adcccd9d1403697d76dfab08871f7589600552afb4222413fe11b52b4b979f191968691107d43ffd8537437" },
                { "xh", "535a83deba17bf72bfbcb521b20347c1ca77eb330ab805e0cb46b1d3e076e63267285113fc2d0331585a4a9e7bc1295e2843191f5dbfabc7a56a30aaf95f89c5" },
                { "zh-CN", "954672016fbccc164816c3a5297606b35aaa6fabb4e3bcd6902e03e5c36688857d8abf20274dcdd43684b757f07a1d6548e113fc11f7df68d9ad77b7584a28ac" },
                { "zh-TW", "7b18af1a35f657830b2365da3697c767a2c5354cc9169d7bd7700accd4b49451c3ebae3c50baebee9476af609a5c7cd3e2e209e6adaf865812cddd639b8f5c90" }
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
            const string knownVersion = "112.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
