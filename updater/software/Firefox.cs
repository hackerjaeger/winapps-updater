﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            if (!d32.TryGetValue(languageCode, out checksum32Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.TryGetValue(languageCode, out checksum64Bit))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/133.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b05eeaefc8098abdf68ce4c8e6318d3c1d91af8fb6349959b3a199decce798a1544ce85196204457135f0f5758d6575147033c433e9401b2c68f01a758895849" },
                { "af", "91ecc8e44755e98f07049d04062433065f0530ab24ca3a4d532a011d0d03aeb308ec5c9d3d7be34a7dd1e9b9dea9fdfca6218a94eb807084ef63a2712c0d80bf" },
                { "an", "18b42aadbe67e46a28b472810faac617859c78ea45b66488e5abda437667252485d7558de30920b8664e47eede66a3b97c0389fd203132f918380ac4a0fb4f1d" },
                { "ar", "e44581a249d46c8a785b0ac3dbea2b1e96a3e5ec920c5b07266b161b776cefb9a6c5f327b560c07dbbec1cbabd31d877f86c4c677da5aea6894decc693df636d" },
                { "ast", "15888f540531b8b0151b8f41f64642951282fb6720775082271872450fac2b0086686b7f55b179463803044353f21ffe101db5259c05324b80dab7b692325eb9" },
                { "az", "bb8b75a3156ce123ca3f1089a8b0180760783ee615210ad55d0ead01a51d4c9ceedfe0b3c27395e12cd9d52c7f5e6dc1830414aebe7cd9b089362fb14c5783ce" },
                { "be", "a0c2438ec5eb545cde3c7db82ac0c7042123efef7c5c3d57b4f89c491d6ccb6c11876bc1e6777d213686e6aef5616432bed3c0e8caa8a522dd4ea660db602428" },
                { "bg", "7f11502550ca96f3d8d7d74b64cf51a514fdb820c63526e2ce384e41483dd4660af5d54714339da896aad2b50cd4bf85de974a3922621cb4e43f0ddf2def44df" },
                { "bn", "0f910e4dfd5201577e9859d3d67faab21cf23baf83c1f74c0a18abf6804e4dc8b4b6e971f9c7d761744096a262701d60e4e666104a958827f1e02307f7c88c83" },
                { "br", "286b63f326a7f1acb24dd3c291d55e8f0eec2d2da701950872fa49c64073af82511315866f16cabc58ed97ffd106c2687b7d38992c814bc2124ab6d5370e7800" },
                { "bs", "c9d8c89f0f9e909bb48211545d9bfd5a3aea80312cbb36e7834b04a473c0f60d714523b9f88096b5d9433523e04731433524cecd7052e7e8b1b216232c3aa356" },
                { "ca", "0aebc3454c31c083ec450b023813b06686fe095d71eb84d07f9db1f08513d8404fe60d7535e47297a4bff246cd31c34d1c1fc55f01a65da3ba43178b2e6a0c06" },
                { "cak", "dca21df0d75e35497f61078a31b09b0c400040f5e74d55d145a4edcbc5d3cfd218bbff914e52900bc6caefd87ca4aaa4566a1847add184049a9bb9318b5ef6d3" },
                { "cs", "17d40721d8ff85f323175a85855c448c90f49f4df8ee8001e9099348292a516e1d0abc60cb92fd49290e23f69ecfc870636e2fe698bf4b830d3d34d2809e5973" },
                { "cy", "2fb0c4a184867b40c0338989782474e2ceaae1b643f8b3a39b1d3feca05d8aa0e53b7399601a9506f0ccebd3f5963480f3db758f808cccf4d67ee32bee94b557" },
                { "da", "fa27950fd4401fb5994105056d6c795f580d88afa63aa2209c39408faf38005554c2a01bc6e0e3ed21166bc2f3cca84ad5e69aa5e85d85a596e6209daa3f8335" },
                { "de", "09a5d157e46704567aa14c303b11256b889451ea5529e0ce1bd262fcff58384a7add60bab841cf3d8c7f1ada372b5abee6b3660013df352f66890e2e25e206c7" },
                { "dsb", "bff7d79225d1f69999c13fb0c82729910daddcc3e6e291ff0639b9620397cc663ab59ac84e69c1515abdc04a4ea00047cdc7b57e52c3773aa4108ae1d061fa26" },
                { "el", "9dbe9d745ad689c1987ed72ca926f15f7828261bede5919cb7d7512e7d167e205ff6377b06924a98a31df2db8f6f5b30e6808042bfa9d668bcc78293680b71d9" },
                { "en-CA", "c6fa0bfb7f3d5651844cfea46703253d6e59aa8fdfe4fb5854f5d30b9992c4eabfa094feefbebf33c3abc3f6ee1c03c07aa7fd0bc2ff60270bcc2dacc45883ee" },
                { "en-GB", "bd4cbfdb6d12dff5c6e375a96eb7311c5769f2a222b2d572e6f067035a5332a786c862a24e1aaeafde78e353b706cf82f39d88dc8c59a82956659f0061033af9" },
                { "en-US", "3f8c075abe30a89fa3b55bbc5a6498ac0958271fa5629b47c16bb932224e088fe0d072681633fcb925a8918f830704ee92578aa8e29315d1db0ba0f9345bc720" },
                { "eo", "4f7c815a64d8d8d40ed048a71b0c57ef9e076f7edd0962d259814c8a2fa96776d00afdb3a239cd67b138671ca9458714dbab8197f72281d129497b85bf9ac4f0" },
                { "es-AR", "4a3873c6df3cbec849e7b964bdb3541dcc6f36e030069dbf860565bf01e919c82b8be353836c0b96f7fd646c15daccb97b163af40a5647ecdbf2df1d75dd1f53" },
                { "es-CL", "799a38aec17e0acaa386874bba2549033ffc95a4881fcdd75942e582463411a90dffbda96ee7e0706fdc333ea5e3e1905971a4e382b2ed9964680aeb98975022" },
                { "es-ES", "19ffe4431229524d07f2906195445ac1bdaefe79070a6824476f11c625ac5a6a424aea3e429526fbd09f83da02e56cc1eca116496cc45645eb02f7d8a4b17a9d" },
                { "es-MX", "5d7cc0ee279d4a56825ae7d981a29911876ed18896ce4159a44c2d3afebedf765d082cef51725f90e07c2aab1cf10df115c7b884b911c9930e7ea7957aac252b" },
                { "et", "3b0a7c32ea8aaa2ce9e67739f154ead3ee5cf3720a330581008d2d45e44e6589b989c48df768ad1e71014ef84d943151a1bcee4d8742f73a3c8b60af91b513d0" },
                { "eu", "dda94ca4d76a3b051889a06da0e284b9c80f9e98f6494b8ff84fb67d63a3629c1e861f22a0e56dfd4f47d31aabb86490a1c343460660aa9815a732dbbf0a7bb5" },
                { "fa", "88ae592f4cf0f9074d5b1d36427add446b2f6daff699d8dba1c926929a101b73e3eb5064996e1df8613a661b022dc3c0b9008bab1f741189e2ac1751e107c317" },
                { "ff", "9a345b10ac9053246eab2b554cb40d78a262db93f266335d0deea1e6605b3eb9fd248ff6b661f3e2bad989c039084458435599bcc574433bad5c79043e77aefa" },
                { "fi", "35ee8583b5a5ed113e26e0b07b1c46b7ea88a37ca31c860f83065e45ce8344ae5f198f32fa41f4b629b0ac79f693dcec97dcd00efcb8dd23f728af98dbb5038c" },
                { "fr", "64f65967c01c4a854458304fad22e34fc56488dd5067e33467963c69131ae0e5b4b31130f4f9caea548661377793c2a66318e9b6bfbe643a67db4d05c2ac8272" },
                { "fur", "c45859687348ff07bf9fa26cc950be1349066b3627fa02dffe7afe5ba313a72b165904ce31cb2790aa0774345101c572e6366a200ef693e1649d8b6a0d870f6e" },
                { "fy-NL", "29bd0daf746542069dda6252d0efb1db5c12ab6e932635c63710c7555e712956f1a9e456419c17a9f3693aa2004b640f1815ebf4304aeb871f9852495d994b4d" },
                { "ga-IE", "3e6f1abe2205e4a6cacd6bd0af3d62705ee97f39da19a9cb7ebe803fc47edc8709e8396e06d9f7c749e461ac91a61e187b6ebb58e7b9ae261b3d23302514fd24" },
                { "gd", "411c7a207834c3aa13789339465f0b6c6a05629096f093606e1864b5593e62e0582dcf4efc5a77dbc4256fe230483c8aabe6c53c3395ac7e45110bc0ca227158" },
                { "gl", "c896e05fe92f884bc6c4fabc77b14c12e7c16117c3609eb2c4753c20c5ff25c84cfc3fb37148465d957a6772e1206b62e9620809ff4468a90b1e9c05a33435e1" },
                { "gn", "39ad16b26f64b48621b0f0e2b7f5e907242cbf0cebea71bbef5f011f7fcd9fc5bf1828d8873da77bb8b8deb37f0254d469eccd17d3e71b46a86ff98cea3c70a4" },
                { "gu-IN", "6465ae15d3eeaf63d51d2fd8d612c0ecae5f91ef2c475c63fea5df01d6dae538bc7d7d2351832d9a078d7cf5206376d895f45f9e12ea221225a38d6c7cc1d643" },
                { "he", "8077da92e0ca148472b463dd28d69366a25843fb2d843428d1000dbed655bbac1f54db38b0a9092f2891c8c3101b01094bc0fad76c357a2bcfd661aad1b37d50" },
                { "hi-IN", "8f9809ccfdaecf304b228ad187e672b3b9d79b227f0e0a9f2578977962934ec249830e7b22c99b1e153f32329eae8bba1cbb156b9fe9378fec3deb844917a365" },
                { "hr", "418da5cd542493ccafdc7b12ab0d76d065690054adb2556b8b530945bc2071cfbb46d7f7fd02b93ebf417e36196562ac59f8aba1c7bfac6953575c599df57329" },
                { "hsb", "40f58b0d854726e6f30e2aa437dc508bc088e2afd634a959072e1618cf2c376a27220c3061e66a8d9f11f3212377816a3481d17fadf00f57d2bd6f713d3dd3bf" },
                { "hu", "a5d2c52f7c7a07b692d267025d5ea21f6abd31a8b1e604e3dc26f197f7deca07d050ad3c14fc27ce031d33f75e07c24a9e06274ea711463869ffc4d5fee19ae6" },
                { "hy-AM", "6ec6356115acd7a8002ca56b079a312bbb53fc0f7bb0db23ed235a4483023218af3ac53ca3c7d5451ce09682341ab2dea496e551c4392314e575fd48b3f1ad61" },
                { "ia", "21a89d9e2c02a7f510ae9809662f8185a973841f0e5fb30a74214cfdc068511b179026872bccf8f99e091362ccf360fa7fa79b52186dfa565c82c8065a8295d6" },
                { "id", "600d019be91baee59e9c8f193b77386b6437c2740755d4dd486ac80efc2fe392437d273f7e9fc57f3978b7e0c1b14782169f9d3e8de407c77cb5ac405e4aa638" },
                { "is", "4ded820a1695ebd59a1bb011ff0dcd7fad3e8ec7e87cf2612ac8a35358ac06d9589f6ebf6058662dacb2ff4bece5dc613466393eb05960ef82eecdfa36408a6f" },
                { "it", "8efde0aa7df7b47c35d692c8c4e47db2f108e5401be54d24a82dcaea845f488f9c3564e131c47e5d017e5c82ed751a87a089683d2076c5b8d6b17fc18fd9d700" },
                { "ja", "a6c55916a3a6449c00a1b116d7a1f18c372c3cbc55d1ba040e07a9482130367f655a5c1d6eb71b931774610995e0815478a2b3b9128607830d2a845cb6360154" },
                { "ka", "188a11042cafb21d4485adf0a3ab705f744b5d82dd8ed3e31b3f8526c64fd7c5d3adce16f24992f59f0dee7a9410523e1867135040356becc251d8c43f67e86b" },
                { "kab", "87edc2bfaf55ae83de0a418b376036467dcf41284a7bb68189cd1842737e6e4827767818f6debc0dbdc8df5cf8190805b6dd5b0d9b9f51eba2e9bb11fa0db131" },
                { "kk", "8cd88d9c11068b78c885d4ed7dcba4859a9b565612971efb6aa5abbed51774ff220fd0ea8b687cfe6ecb37e8b5004cb335f4633689205bd4ee9352c6a142433b" },
                { "km", "873e5e4d457077fc193607b99f9333cab703c801c03d2c94d454edea6f3e1cad50018126433383eafb57ac57164fef9ac729f35492b1eaec6741ba478718e6e0" },
                { "kn", "d0c523c64f45aaa996cec77b513753d2e9884d043150232260b0e481297315f71b779c3baad4000bc0a956f8cc3302a842841d25269883df058d701bc0c6b77b" },
                { "ko", "de55d8f437b28b26d0c6446b155eaab4abe55817298bf0e49bc7716cd1975f28b995d431739f93f6abfc7f9a6a0a7ec68f317fd9911fa542f0706f3d1b76c77c" },
                { "lij", "c2e2a0f6d83ae31812ced6fcccc929c6ae11cc26d6611de7980cf66b5338e88a122f24aef0c773576374483bb769fc8a2881fe197991c7c1f504645f651965f4" },
                { "lt", "a0b9ccf8e9777e6d07ceb540fd2649fe05a69014485e542c03f999b0ccb06736aedab1a212e852da51dea52fd2417f5456cea20a42cd20b22fdcef392c19fc44" },
                { "lv", "065e2b254ff49337ee4b295157879dcbf5f74078108742f36095159e96cc32ce7ea93090ebcf61add5ffd061ea6a2c4c9b4ccd14398459145cdb9cd33cd3d1f2" },
                { "mk", "f82110153c7b7ee03ef3fa3314b6ef6247987f296964d1d63212e71ee35cbcfab4e22c395898e9da87157ac96a6e51471b49555647d7943fa6ed3033ec6abfd5" },
                { "mr", "0412a7aebe8996c17f95a12f08689ee483b6416de67b16ba9d9bfa8cdd83a923e6ac90ae261910786747f87f5e099841928bf1c0110b00d5d4a4fd0ab7639b4b" },
                { "ms", "32a0710f5b5c0beb6e1efdbc13187d73af0b7a4d4cf9432a45c4a4a0045566f569dc4abebe8624c88397017a47cb790a418325d329c00245e4c7644a92c5c697" },
                { "my", "39e66741409856c99b8d1e020db57fec06e71ee762f07daeca2cf11e7d4f39585ce7c3d5b8200e403ea6678e82ca7a05a695b9647f41b935efec81c4b1ac8e86" },
                { "nb-NO", "831b41231a3f940eaf317888cd53e1907f333cd6f1ac799f87ed3eaaf733b638fd263ba98d14f3344ca47d176dd53b01c1c83581d21ba0fab4cb54518be4871c" },
                { "ne-NP", "bc8aa1df1efc5395056916ec9298b3d19f7042da06b9994b8f0e04d028ca812fb358662458cce0d6ca86218a91376819df24687bf94cc05e6fbc089bb84aa514" },
                { "nl", "24465ef96724b4c8a90b7b280395b671e338e609de45c9b5e7ab7dba321f85ea493d1fc8163ddc06f87a02dc1a2550739ad9dfedd6561bfc18d33f53359a3fa5" },
                { "nn-NO", "7511beb4ce022ac86c2f7ebafa13a3aa523571fbeb328b603031b3327be8dc64b4b5536e789c8f9d18412b0c09483e3e4b5ca64e2f0cd177ab9cfb2fe7e5ea99" },
                { "oc", "bcac9b4b60945d9fc9a4ad0c431829583e6ca48cc21638008b2283ff93e8193fc1f81e5be3da7ddf0b5281d9d899bba80a72e7fd0cf94d6246f14f8d552db59c" },
                { "pa-IN", "d72335bcc9db4b975e0ffd7f38f0ac05e743472b7ec36dfdecf7bb83589e185efb8cd5cec7d9fbe801512565f7662c3aae5ad7518c6b0ed3658152482dad4d3c" },
                { "pl", "dfa4cbf1489b9c2988422cb15376cb1e4f628b176f3314aee49f6c9ae28d1c0b31f7d794a73fe7ded7f5e6af92dad3d458510bbf0fee0d29b23d6c06bd78af14" },
                { "pt-BR", "50d5e031b2c30369d11008a6c40b0d6c58acda8fb2fbc24196da008881f3fa030bb652c6c9f233009d1eb00e8c21f44d952dc066cfdbd7293a2b96156e306459" },
                { "pt-PT", "7b971feef821ed24b03bb6b005df93556c3c1aaa1fef7ad935214b012fdfe8be0dfd4e6d4e645758844e5cbd46928ded5bfaee85da8d0c331ed0f1d00bc84a4c" },
                { "rm", "359b87671d3ef1135543ba2af50234de0dbdc7c55fa134d203f216ed461c9a3f60fd645544cfd05072bde41c3b2efb4e4792d98001394d0754c00f7c605f7441" },
                { "ro", "91e05579215044eac972ff341fff9e3f92ffe336d8cd77392467df78373113dd65f328ff5e6efc56cb5e0d75311a09d6ac1f5c2e0b433e588d90afa13535a3aa" },
                { "ru", "f3fbca52cffd9a74fea927416f39f0d40b33fecec87e235dd214e5737162dc31f0cb87c317d69a2b13cffe79faf775f3be23911a4e01b84e615b5693e88b8227" },
                { "sat", "e1b8e63c24f15c1adc101067d0f13b5bee94096e055da77134e740db0a921f41fbe384d8b0ad06be6be7728e1f76ac84752aba6f4ac6883c2e4afbf4edfba673" },
                { "sc", "48be4376c131dce1fcf72181fa5f18143b8b222a6b4ff5533cccaa693f86698fd7d515324bff60c1116ae8f1a9fe3362b5c232d27632ec65a0a892dae2eb2505" },
                { "sco", "2f97cec9f3ffeca1732a80fcfaeb889a8eb150cc4cbbed21d1d3b81054c2f202bcaf6b9dde6c95f25da8dc5a1c85317f946dbcb4d15fb73fc0437f44a45d27a0" },
                { "si", "23dc4b5f2cbcd0e8234e0dbe719a46ec3cb933de68433210a93d189413b68280cccb20cf267b3ac3874e91514b24c80d6a9207cee93bd7d9b22920a51b24ae26" },
                { "sk", "6b30d8993b0ec0aa308a72e7b1d880fff6b04145c92419dbea3aa5bdec8e5599cfd6060b91cc8c988d617a6a83fb3b9ac56d7502a96193c9e15434d5fc5b4e74" },
                { "skr", "87aafd603dfe4be6e4e320094d9dd077911087b6d51d4f0e18ee15c7427f8bad057b3eb8b6c01562d3c15e2e4b49fe54a1a44c93e0add5fa8ad7ffdd297a228e" },
                { "sl", "04b5f77f05fe7ac82745cd6fe71eacca436fb9f2475c4b30ebb9b6f669d68e12400cebbc60d5d70d5cb0c9a28f1ec48eb9d1fafd42195ebd1673bd29ae96d710" },
                { "son", "0db180befbc4a926d89c6b044469868b3c745f6cd32a950d481478203df679362109d9eae834194322e65d5597782cd743abdb183511d1712445d66e453aa8a6" },
                { "sq", "8e8b9aa6630f07120d9305abd1f6096d594d604bea6af63e2bf2e4e9c63b52451cde4b20dc169dff26f40d5198aa3e63e18671928853d1e85810a6c3e6bb44b0" },
                { "sr", "a8e4f23d9d1c2dc945c9c357d07d76bb2a0d79db362107df8d516d429a80fcf1ea74fbf1e0d28e1ad2d6b4210f4bfcd2e56c595b1508e1be4bdb07a5f91a4794" },
                { "sv-SE", "6b7bd0fba6048acbf42b51b122b4aa0d4412a26e93e1110a57e5f14efb128ada2c43b152ce7241bd5e5a81e96c7205572dcd6023c3a307dbe849cdcbb198a1ff" },
                { "szl", "e110a1f528884b7847c3af52d36c4b94161088b3d40fd71b386d90e3c0e06178d86094e2fbbd33d039853801ab80e518c264fabba77a04bf7e6018762c703fa8" },
                { "ta", "7a21865e6e385153609c52d870dba12c52c566af50ad1e4b4b2a5677f86a26ec3da31faf5e44fa761ea94a4efb8d5c07ebf8cf499ee460280ddf4ce464e9702d" },
                { "te", "002578a514a0c16e7980c0e0dfe0d566503ae0cccd035eb3e1d7dab4f248937cc2013c7cb2d72356ce77ef447b3d65c2cbd36c48b34d0160c4c1a2da1cd3fbb4" },
                { "tg", "8438ce6be5eb0b575c0e73d4c39fa3768f7a00d2a98a1320e2a1c269a16441c9a80dc438f8d9bab0ae2fd10c3c1d0f19366dacb00173db85c2c90b36c8c77f55" },
                { "th", "2a867828e1be7479516d1c1b469a6db830e7e370c69bd446da27b09b3482f5888c9b48ea46174127a7fce85372c9bee016d519f0bcc9a00f6af83888a19585dd" },
                { "tl", "4bd40ae127a24b5c5687315697db99b4ea9fdb4f6bfb10c0c321c13c1728eaef53dae49abdd3906429f6ef7c19bc5f1b23e809fb184364b52151f7436bad0b4d" },
                { "tr", "eb3d4b8fbaa94d469e2b5146619fe0e64bed4be60845fbc8ccd2a0a98cd22e85caa66e0861357aec4a9259f86c3d58b924ab77df92094e11ef61e4c8abff4ef4" },
                { "trs", "224efac9628fd077c1ef504af6d9a87b162984415cad50676338dccd0110869c3ba5f16fa8caa1c60f8638e1ee1e922fedd954ec994fb043425e439c435631b9" },
                { "uk", "608812b2e6785e5dae4f0c734231e01b2178a895d380784a7308ce4485795866d12ed997fc098a783f5bd3ee196196aca9593be2edca7cb333cb0e10814345df" },
                { "ur", "3af0af82e5a2436b1ba86c6605909d283234e31ad65a146036273eac5b1edfadc349a4be9a4e636cecc9313e473a4d7e7c825ccd5cd8a68ce69546ed2e28ffac" },
                { "uz", "480de0daabe8d5956eb90b2822203f9feddaceeff40ba570a77f961684f2d825e44c63b696a728a205306425f749c86ed53523b70c7805254fa0a9b8102eb081" },
                { "vi", "a763c8638ff82a9e14f730a72a8d331551599647593098db92e943078fd5a1c0dc9daa314aefd07ec70473ec286799507769b7df21203b65e923ed978fe58a63" },
                { "xh", "0aaaa7c500629a25a4fa5b1ed2c22e0a9e05783f92a9fe26296596c72e1a63ad1bb6c6ea674c62d86c77f316ec6c6e22e8c325ec04495a67b862b44dbe4bc497" },
                { "zh-CN", "aca6cfc3f1bddc9022b87bccdf2664d38913495d64ebe797ad000a730b74eabfb83c4570cbb2193eeb5291de502de5310065af317af69959291e32fd0ecb9110" },
                { "zh-TW", "f8227b8e16a25a17c11bfb37d0c4430e6dc65afdb95aafc9a11da4b282c9a456d5c6683afdc4a0705b4ff07a39ada6c915eb021b661bb122304574f4343b4000" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/133.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "2cbfb0e5e94f0e542085bf83c46d7cc294233c8c5df6e65311765e5410d4b62fe1ad93d4976272cf5e7066ed2cf053888a81b2639946d1082a64b08acef16415" },
                { "af", "c6e6a63db1c9c18deaa949545bdb22b83f94b58a2c8e0255686a212d562e3c9763dfa63ab9e08d2566333a27d7cfb57519850edf20fd9192463c9aa109e81a1b" },
                { "an", "b40c1811cde76dd48f6bd9a0ed6231c6710b7238a0a0d5da810c788d2a67a7dc0bc47a715d215927d2b43a984a85a763f585d0a820ebaf52df2f1b6b94920cc8" },
                { "ar", "6406e889902f0f8edbecd51b82ce4e4923c955a6ed4de9ec2553da629bcecb34fc1ab6a147a369240f11871b9ad919f647206e876e20de1fe9d95382dd5aa9aa" },
                { "ast", "acab842e0f5db6d1182082457de741d301b063119fd4af3eedea473ba9dc1aa4d9c0dec8b4e6f8c6b25d4c1df6d38caafb5d6a5e3990c192db510aac3ca0e3a2" },
                { "az", "2e4a46f339e9189a0f40f6e4b0c36666ffe565d48ae6ca5fb9f89cb84588e7774ece6ae0b789017e977a9d5a97ad83dac7b1bbe5db1b19c8964963c0c505e3b0" },
                { "be", "451fdc9f1e33dcbd248832e7247dcfee793df10eb923e8df0ada2391aa63f90ec8a6a971f45d1426f61bc6bfa5801d73255d99a2aef0ebb2899d6d9bb5fcba43" },
                { "bg", "e6de934f593242e39fb5af757a7c6b9935d51967fbbb79f28721451e89f8a07a6577e870a507f10ebef4da2dfe1c8fe3fdee3dd0e4d8f2ca814de7640e759843" },
                { "bn", "be18feb28598056518ac150c74e5d8b6efaaecb14cdde97a8091757fc5fe3e1dfc89878d3c185d16b1ed13fc031ef723e83d645602f74ca2bad2f578619c0708" },
                { "br", "9b9f2fbbabb6de7398fc003b22f865a506ea4d96d5c5f30b322a1fda6c6010cb03a80d8df8a3ccc8c942c4604fda5616090ad49d8082b284a1bdf93f1f80567c" },
                { "bs", "54b20016738d9fa82778946ff39babf346473e31e6f7b1df9740e420923883f8e96c321a4849624002d7835c22d58a207800752018c08b9bee2b15eac26c9e8c" },
                { "ca", "00dd39534b2efb97e1ffc8e2005a2d96f78b8a9ff2e2a01876fc2cf6925e1b7cbbc4e30754462edbf6b03328bf3af055fc5eb2593eac653bb5979c897347de37" },
                { "cak", "6215de5506765b7a9b82334c9ce53f6ced3931a710cc6ea3673269892f36bfe5af9f98925328b446a2c6129c04c446f6ceb8264d3a219ddca35ef19aa4a6e8e7" },
                { "cs", "b39e22befcedc288fa915919a41fc4783a690b57d47b1ca49bd1434e78f519c2d887e03f368742507f63c4529967684eb0fd7cd457117759968c12d3a63f6fd9" },
                { "cy", "cfe141a7e2704ac2a8ace23afb7f71bcbe2bd5924a01f34ad39db29545be63f31ff0ce5481f601ecacf9c603b6a83c43bc85e6a00f1a77b751b1e482651e6c01" },
                { "da", "cbe9f5cadc81331b89eee4a0b7fc98bf4a71f7acf77a9de00280fa46a97d381e7dff8081f8b4e251a9eafccf525bc1f1ef27d67816592ba35bb9d5231a9aeb8c" },
                { "de", "31da45590ef2900ccf9bcd3d18cd2941e7643604c3cb6141e00f6d3d1431fd4a055c149e5c7cadbcbe0be821acbe051e3fd7d9863cd303e065d5a31b3aa567fb" },
                { "dsb", "c3ddefa1cb04c89066f393b295c9e5cea0241bef5efc1358136f69179249033aa3d1a59b587d40ca88cd953e292ebb31c732d8e14c999bdab1c41665630204eb" },
                { "el", "c64a11c671648c5f5fe34a4b8f75b29ce4aa1c6639609755e2a0cb0881547f8387f6684e53cc66d1c49b567870251c35f4a3e5721b8202dc5330f1713198efb1" },
                { "en-CA", "7b22bbcbeef734d87c035101b27594aedbd0c042a86a7268a5b505327f2f97cc4ec1d70ef4faf7366562edc2c1cc778bd6be2a1feb3a9edc2c80405022fe8498" },
                { "en-GB", "7dab081b2c61fddef6cc788b6aef8626819c4ffb79c941a9ae8f456798b1df1cdfd07e5e368d42bc18363c3dbb8183ee79d1eac8cd2328c89229fc329fd184b2" },
                { "en-US", "c77994c590e61e7104a7f165de6dd1ea207959d132b5a264677035ae593823b8fc6ade038a136dcc1ed5ba935f51bd7820e89b75ea1a22163c49f89a8d0a83b9" },
                { "eo", "efbb2fbb15cba1f5c3ce55978d0d0291dc2c2b717f67b8cf1b874777d6d7f7f72b6fb429cdea4a145d2427bf3bb128ffbfc6b507162343b30960566d64bf6892" },
                { "es-AR", "69f04686aa3850dadda8c2ec6d992a2f57296b790435d04c46303e4a1c3587b9f1601b99cf357e47722afb5a43accd295cec7ff905b12a180aed8e225f209512" },
                { "es-CL", "f2950b128693de47610033f81cfbd3c535927144121c6ea0041bb34cd485aec636773ce1229a3421973158600d3d2b09b0b476cd99a93650df3931d39efe42b0" },
                { "es-ES", "bd6552d4d25cdfad18c9532e5ab02658c08f855d5ae596c37213c097c1cbb03201337b76afb243e6172b4df991d6429244472c7238fe0c1d849c00135a3d1d07" },
                { "es-MX", "3236af32f5216ab499a9486ac84321214b05fa6f9862de8a22725e2f3ef09bae0b658982114f8e83abce7889d69a6193b1714a5f8c3c8f4b44b054c016df3310" },
                { "et", "4afd969206d78be119cbd94698262b534eb51daaf7a8bdf52a367391d75013f54cd1c7ed34f0be35dc4211501a2bf9231655de72164b4ed75bdf3934f200c208" },
                { "eu", "8ca4091716fa4aa7269aea5e496fa84800445e1eb0f8592a0e20f7c477565d33a7893f717bf608cce016ea2a885fff21aeaf40d0f7428fa7158c2e21dd2c31fe" },
                { "fa", "31c62fdaa5b03e69a408c9b9bc1c1d0d489b5baa4cca420e39f867ef90c0ab3f6396d1c34646da413b11074149792dd3830140da9f237a0f4f1ea11f2a033968" },
                { "ff", "96d9c354001ba74d565b3cc6657e3a2fbd34062294b29e8b2c03df87ef98df51bff71c15ac17acc3f32b4890aee040473219def4c902d6faaa85c4ede3ba070e" },
                { "fi", "281c0ca5077e8b62b9e4c5959eb4189483b225eab304a95525c51c712324f23514eb3359282e9a41ed5b23215299671dc77592e9ea3a8fc58c2063c330da575b" },
                { "fr", "d0a5aefe2c5948944fdc20c3d6f487776e48f4b2accdb12fbfce94bf21367f9642676db4cfab55f77ac83832a74c86df05742130a4b880740bce3b19a76ce1b0" },
                { "fur", "85315642c017ed06d70475406e8c668be70f582494bd242ba10996591f6a58fb8f031fd37d25c88cb3d74338e29b66d7885f8a7039185cba7ba70417f19d1572" },
                { "fy-NL", "0f1d2ecc9fd399abb1b60b433de2b9f588351158286f7da2edfdd5d148b7e1d06a0e0e43b4125d600ee337b308afd2a2b371b40935ac5462ae1de044d9e5346a" },
                { "ga-IE", "7955de473bfbe28d37ff63b3d9c451d271e0d45cacbf94c575f7ac4ca28251b870fb9779672b40edf9902c09185aa7d155e9877e06e2c71deafa90b949cf3e17" },
                { "gd", "8b8cf065d850b855f85a1967bc0c92dc3e31f57a9a14e60b522550b28bfa3b02b0a2c47a349e45adcc07c34628533915c86a04e3ceb40ffb5b9b222b731cb98e" },
                { "gl", "2c6635d02dd1001418c23acd79e1beb103cd2e915e5ea664c0a404cc8ba34f604eef6f1b9cb7f7171441838aecc50c4458d4171606bb3fbcfcaef976da03f553" },
                { "gn", "16dbcc4f933ab558ea50e51c7edadfebca78020c8d3f8c9072e6e389a5f59158cdd208cbd6421a21dae544f8d1642f300ce0b10345d1241bdd4b51745f4139c0" },
                { "gu-IN", "7ec19ab39c2e3713e61db736456891153a426b2c34bf41ccce17c498601b71efa0578ef17c21b5317f335f9ec543d1a1208c29958485ead6946d5620d0dc0429" },
                { "he", "c2b310cf368a089f1eefde725442439a0ad89abdc663c662055e13c6e902c75028eb66b379ae6b0f114fa38f9e45d972328792cfcab40d1905478e16585b2dbd" },
                { "hi-IN", "a3bf9f21e93fa138376f9bb961d076f39b6fed87e5df8c9c59a88b3e7c605a694d02ad2c44c023455ffb7e53519f623586669b421d95092e5f35c72d838ade6b" },
                { "hr", "9c6fb99cb2d87b36fd512bf42bca734fc513cd55b2844a5ebd9176f4eba762dd4201476f92a892684b3a4481b674a879ac91cb2e04ac840dfcbbfa1bcec6bcc5" },
                { "hsb", "8d29aea450822106c65fea29fb573ac044d4926afd5f16ee6b49c2e967d35101e8f7facac34443119c5b14ea9f60828cdfdab3acdd876525a29127a1a63e2668" },
                { "hu", "b3c67c09bbdc4bd4155dd9fe9c6becfaa77e2909cc852f5a12fda4ec32c351a7357466b3e6a9ed06b58b7e4226eeee894dbb9141c17bacac76085df0270b098b" },
                { "hy-AM", "35ffdadf6a280b1c57e9b36c85c74f28ddd46efaaf623db60466b3198da9eb45e8e4229d88a6422c50c9979f96b97cf6fe1fed3ef23dd6b057735d1f4691796c" },
                { "ia", "8462990a92ab765beccf81aaae41d093125c110a42576496689771f6b71d53cb50a2b0e3346bdae4a13921f4e45420fd93cec0976251b21e13feaf5a577b5524" },
                { "id", "fd01cdf3c4ffa4da1053c2f20d2351cfc273be232f80733408d221b1f01e90827db3294d79a0555fa129bd688cdb4b68908a43a2ebab216901a6df995ba67cae" },
                { "is", "776f91643fd7fbe7aa0b088ab030f21a99ee97e483dbb34c07c7cd1c9019bac8788b5d822b08a52908cb371c8b1f69217ed68f75f5e46556d74a355bbf57482e" },
                { "it", "c3c4d17822e1440dafbd7cf59a9fd6d2269831d844a95489d9184904587796f10aa2ec07cdcfeeab08365b2b7e363c4369ff8312e20302c432fa5c55b13ffe4d" },
                { "ja", "8915b19b12c16e4b9af85b303a317c17401d7d27bf425d63fb3691c429bbc75deb51a0717d55ab22541df0a61f04dbbc95fa2cd5934912c0f26e77977686fc6d" },
                { "ka", "b3b4b3e3ef132033ef70eceb58388a812826065b84d592b642e9fc6081553dd5d624fce69b65e952782be9d55c16bf1a860e4826345ac36e72577b0d90dc63ae" },
                { "kab", "e6f9e0ae523e8ed55b53a1a7db34c46a9cbc1e5a7b94315a4365709a91ad6ae8ea4f255873ec52d953abd3c3d04eb4006f9b4ee838c40d4caad315990a31c71f" },
                { "kk", "bfdc16e148b0c60abb4f9ded16c898109716931c8a432327d392eb5c7e9bc4cc573bcfd7a752aebbe656ac92980da41aae27f356f5d884f3c3fbb4469037ea3a" },
                { "km", "48421f76f9abe8c4996bd0a2bd3a3ef8377e41fa0fd9bbe4a8274e7b69d9ab28ba00a435ec678afe228c181e117670b8bb4cdae636566c7e1ee23d8d096f6e7e" },
                { "kn", "67784bfa96409fc2a0fef2186f96c9bd9246ebdf30a3d1e8e78c1c394cf2b8c874b78a2c1f1a7196708d3efd04ff3487fa3e4d60dfffe8c9ebf8ff994313d89d" },
                { "ko", "8894deca68ff053efb8ca85669a259e37db5345ec88ed0421272f36117a9c3e1dc2244694216d4b91b353f35ffa614633bee716d650bedacdf6fea819e2555cf" },
                { "lij", "698291f09b6bcf52254c4af3b5fe72cc2f8ad944e05190ae3d67db0fdbea296014da0d4d8afc7dd115838a5fc49ffc6310751981dda63dd1bad195057d0ffee2" },
                { "lt", "7288272e56e2905d169a5964dc98f7d50d61484bcc2690d156f095250daa1d00a4e0b42567a8d2590eed61e482b0c4cd5924e6c3514efc5ab561af39bb5bc7f6" },
                { "lv", "8c4219f92c7a55699cc43a184e06c7f4bc4f3ab7722706c84eaef2b8cd43f6bc2fd4827c8a109f5dcb60384a733d331eb549ba1213af23c371edd9bc6156fdb0" },
                { "mk", "3ff436b9bfa89ba5c64f323868bac6342173ca3272b6aa815cec88a75b2fce805143087cea9b1e02bcb855a11c0765e0814be99890b8387911d7cb36350b1ebf" },
                { "mr", "2cc1d7ac0d77008d3d9802e1ccf9411ca60e472abc6a4f53ebcfd2f72fc1e9d0d89aee2a4b4764b21465b6ff6378e979acff83805a13f7bdd64c9c0e75b521a5" },
                { "ms", "ed53715db7a40797b0edc5642cefe045d668ee158a70053641dbe6b98433d1f4f013f0e0ee78b198e04ea5b765d2028d6d354c1326b3ef00e26e12794f273061" },
                { "my", "51de85279a5fb59a9417e05159f989f79c94a210776455d68889afc9a7898e4818996494ecf4aa0863218228aa030ad223aee65fc2a601632b45bccd65e010af" },
                { "nb-NO", "f554a6f5b26a69e6120c99732aa2b024aad0af817b048d57a3520fa3435cd1eb6333ec35ba59975e780d7fe6dcdce9d5d5579879f760ca09a59dea80885b64ee" },
                { "ne-NP", "a3799b429f9b79ad2648cdea022ea18904fcfb88fbeb5cc74c7f1ceba12b6d39bc596843ce928a2b297955583b52ac5486e00de7ab6841162a495e92a33099b9" },
                { "nl", "f5dbac3ab92eba372ff883d6cfd54d29b8875edf568ee8a4d4da7d427ef9f10093b6ae18a67623d05d3aec242e25d46c1c85a4a3ec9b542e3a83126cca5ffd00" },
                { "nn-NO", "06bc377de637e22c65999165c41e918b9d418fbbf38a3a1f09d23425d91b9984474ddc18ad039f461f754655875ea30ab67c57f2a66615f0bb49cfb62b8a547c" },
                { "oc", "e30ab043b3b754ac7e4b8080cf24a1803729e03a0931f0ecfcf536204602be0ad6ea80d9c8b4916eb9f5a5c499616895185a90155600eefc9987150643070fe3" },
                { "pa-IN", "6d87bed073ef146f142c03b0dc64c4d1df4d40006d8d79995e0842fe4e8bb1e8ddb3db5a7cfed07614f4f4d8f8e59a9353dbcb76ca8ffccbaae555c29b35a427" },
                { "pl", "1101dd6c50453270e60c2252a04db4da99b7f0a9727814811017fb3910e97c19e6541d6610e0a580bdd04e98ce6ef72a1ecb77d92a2a481104e856a0c4c9de0d" },
                { "pt-BR", "2625f40691679fb70e70a9d35201a0cea49903b62dd65e13e86d8a1d55236932f0f5d8c3d6bccfe826decdfc77af550ab1dd06daf665c1d629e95705bfb93f77" },
                { "pt-PT", "f745e507544347e065a5e466be96ee6b7758b404fbd70ef87ebe573d06afef7d763f878a4f53f241a89ab74a4926cd836e6eb4c18914a60f493ab9311181e7e7" },
                { "rm", "774430942409970700d7b426ec44b31065e43f97d14c3013d7700952b6827eefc3defb273c871cd7a926f7c09d0d6dbb04fc9eb8ceceee5d6596a264447a337c" },
                { "ro", "5ccb572e352c7d211f90f096717357bd041b324ebc5320dc83185b16e32b3d25c2efcda2cc576dfd6ccba9d8abeb6d6651e0c15decbb3adc04baaa3b66b77c7b" },
                { "ru", "231f8211a164775757b8b06ce8655c7c584d32da15a21c45fceca3155aac194bae5156e553f90dfc25aaac8cfd438c30f75a04945918770e67ed84ea8aba7e75" },
                { "sat", "dc2d196f746e32289ba6f723fb59efca7198bc2ff0d4f606362498a24454b92e86b6570143d95e3189e5fcbab44105528d5a36331dc19ef2d4c3902a10ef3da8" },
                { "sc", "77d8e63229fca784c0f132d4295f6da3b7c435f6f61080263b686c44288da78c36dba81a41f422aefd13097735d93d424a542cfff000a89cadadfce9ed9e07b6" },
                { "sco", "cdb7abadf9c78c9c9b5049058291164b2be461c19a6c5dbfd9774d0edee82c88e25ae958251ac87ec688fa923f70d24a18072e88c562f9a2575735694142fe88" },
                { "si", "565db0fe657441241104df12b6548c92245a4c454f38ede6d41a258e2b5359071d348da606a018f7235f6c4b60e8aa4328d0012462d59b915cd6d081e65e7f6e" },
                { "sk", "7734e528f3fe20800cea1467175b7d6f0bd14c78f06289a373ae8347dd9453ffd52436e40d6073c815e2eaf9548cacfd652c686f92c4bcbbcbdc51451f666782" },
                { "skr", "4d44b8cb0452c8f3ba3017cd941cc6c21ebc05215fa21d0bb577e7327e99979f7adfa74522a7362aaa59faa537b211d5c397353e5f6892169cadcfcc4fe8b508" },
                { "sl", "20ddb9e0ef4bdcb6160e1c42f0f65fb62ef0315793751218e9ce30737b1bedc3633d5425fa68d8c7ab3cc8ff8457b97c31de5088dee6d8666c5b2c06470c8a85" },
                { "son", "58e3f3ecf2153b297b0c584c4d4df14192204e771f6ea7728ac4b8cbfedb7306ef5223078a3bde78a283a593bae199b88b13814495418c5e0c351428de705eef" },
                { "sq", "83f7f5a2a1ac8797b24068d9e7de331f28d52ac696c9ae9e5f89372422257cc93b912544832b8eadd027aad6d9e2d0b43a9f47e1f3443d937265434413fa1375" },
                { "sr", "bd2abfbeed0c201724b45fa88e6f3f10a4ea00811f160ca399eb10834218cf4fc5cfab5e39ab1b9fdfb3f2102d804e5e9d5c6dff2a457e1a30c31bb86851f890" },
                { "sv-SE", "3fdd7a63da1fedabc51da709049968afc7ba40e8884ee57154da445af8402ad1d1608271ef75b0e59c8a21cb8dfcb8762fb7ce077b331c2163b0f2466aefc3f1" },
                { "szl", "0b3eda5dedb947dd31791d77b3d2e68bdc59893d87e12bbfeb74633500033e3f05b25d49109ef0a49909df531924cef2c0ab4a8321eb0e4875b2c39cda660926" },
                { "ta", "f44f64a41f70fa07433e64c63f7a26ad87c751abae4bada5f46d3aaafe61d6b008d684e319d1b9dc160ce98d98237e909e0921f0050234448a3a35e609f3e3b1" },
                { "te", "17b7c884186d068426d25c2e66664d66b45e0f1fa5519fd4c840f0fb4b49fd35f2d394111ab77db4baedc4c9040cc5c72ea364790475880c32e1076307feb778" },
                { "tg", "4e0f5f5bc700ca5ed00bfa5ef0ff90cca6c147f36ba4c94f960a0e42d240f75dffa78922509cbc72c9bf1f279820274788b53c54c24f1dad2d72a5ce8aae9c51" },
                { "th", "50ba6670e5eaf7b2de27cbfde0559dbe6e52c1490dd5c4964b186f280db01e8f35e2cc6b760ebc0c390346e406bdf5e35809bee891430ae64c8126d3159abc14" },
                { "tl", "8e9ec39d353350b1a4bff2e3743e6570933b298b635301fedb878250617a20a5e35ed72681e0bbe1ee73fa503f0a7536818bde6b45d583239c35619a2a3b0716" },
                { "tr", "a8b87496e00f285194f52b1f0d6016daed3a1bef66dd8373bda6ab0df6a2cdc42372875f5820c2cd1e7648740d2075da698c9b3dae6f4b2f9f0320837362f59c" },
                { "trs", "720c0d2192aff7aae0c54a2c9abaa42dd6671507a89100309fa03395e73978ab3a9b2eece4a7a6c03db534b7f032249869ac1713ae9833df9f3974cd8bd8d2a2" },
                { "uk", "35d311f845b782029361a59c00fe3ca74647712f1766185bdb0242b284db054b21fead7617132689d0c20564abb86459d8187a3ebf0488b8cc18465a45e5dc72" },
                { "ur", "ce588b80fd5200dd3f66e36ec99095d1c342eb6338fea35488b6139ecf7c015df14d0aafa4a3109eca31438355189c39aec1135aa7aac0b3e6e214d597e3ae8e" },
                { "uz", "04635aeddf3daa535b245051be751de54dfd8d29f963147aed8a85f265cf322167eb2f88326acb5ebeb142c54c7769c300e961998d409d5911576b5ac3dce49c" },
                { "vi", "e6e3ed476d0c82e79bd087e7e7f3b66089e8a46bd4ea911ce85a735330acc92c052428ec3075b69f85f405f60be23107d023cfe41671bfbadef398cfdf1fb141" },
                { "xh", "a83853a35458d366f666c6d29b91259ff99d9153e0bff7e78f4ca1bb9ebccf14ef3fb71e2088245333fcea6f7a534a197f05d728a2f59ed375011c2b0c62a615" },
                { "zh-CN", "e2f706d53b7199ace0784b352af46aeddab1f8259c32e59a6efd31c51e89e90770797a80dcb5f4f9c1cbbd54d8e73f3c19da0ab5996b250011fd2f52c5c7c5a1" },
                { "zh-TW", "35fe139a2b9238dd246bd9780715b46c03ae020701f3a0b0efddf4e913ad45a8b2bd7a119e4b47eef4f1fade137908b8e6636bb4853a4df8d7d0d32800285c8e" }
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
            const string knownVersion = "133.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox", "firefox-" + languageCode.ToLower()];
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return [matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128]];
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
