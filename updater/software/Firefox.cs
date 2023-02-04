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
            // https://ftp.mozilla.org/pub/firefox/releases/109.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "63cbefcd3f4e4fec3cad21f4e4634ee349f9cee05e34b2cf8b54ee69101008524bda4a6406e929c5202176363fd25023befb0abd6af59274e8cbb71583bbe736" },
                { "af", "1cdab3cddbd79cfa716831720765702ad9a28e0146c84a20af8d21fe3958aeb9c68cf9d968368ab9a962b30fbfb251be3a5d6de904cf294d1f1b8ccd9f972a44" },
                { "an", "bd7e8100aec3b200806bcafdfa96809008a2ec2665d9b2fe676e49dae325d295b56d5b720252442c2c30436c786bb742a48e596c1a657e07bf1d389a03bf7093" },
                { "ar", "d5116aaf30c287cfd4fdf8e9e7105c25fd6031071fd6cfe85ddd3888c545aa4d9b6a310899e02033fbf955a04984c501c73cdaf06169a9d70ca027b46ebe4391" },
                { "ast", "44fbd886e16d9b40da47741b9910347fd288d3dbbb1565f6982ca00503aff49d87c628d57492f6857ae639bb897cfe74aabdd922150144293de07f067be3b0af" },
                { "az", "4fb7f6e382a337007fc76a235d789239f1f1937e476fb5c3fa124b37f526b487cb9c65646933406e64436ed33f63d45f8f6025cd09a84a3c1bb77a5d7021d9a8" },
                { "be", "47f76020940ffdaef9fc0c1659b72bb405bb4a5cbef1332dea1ac2002f49c960f5fca799aa5e94501923c668a308aa454cc19f2cafda5bf9f80ec275855ab67e" },
                { "bg", "fb9b9a895a9ae1974b110fa81b91bc263b64acaa815bc5f15d47adc90c61260c95034ebdfc3b626e50b3f82b9e56f63185e0dbe06173da8016bc8a2100a6636d" },
                { "bn", "ebac367f860ae1548e8b2446d2baa20b9e5094cd4dea98eeb7b105a1cc0dca252276487ec3ae7b90c202ebf394eb7da665556c77021f18fb0f13165e9daaf32c" },
                { "br", "497b92e932d889387e64ba71c025a2be33ce46c08e0919b17b45b331cb5f587b65a2783036d6a817108c491ae23b8c97fc96ced4bf44d2b5d0bf332f97ee7528" },
                { "bs", "dc0d571563013b7bb5f2d3a3811c68be81d3557d9f54057adb18a621bccd7f37c6abf0b0dc464477eaa646699081056f6e6699c9590c81d1f209fe086e042709" },
                { "ca", "25a6876286daec571afb318dedaec7f79d0bab2445c81b65e966b0777807810332a886e825e495ea6dfecfe2981aa0aa7a7054f2b66223462f09d45c4594b395" },
                { "cak", "fb9536f34f2bd3c3257ab7ea4b1a84813a8ec26bfcabc893853b20b690000a21a8b70e2139d0614e64ea37dea4cd1295d7cd1d73227143dc011436e9be02d793" },
                { "cs", "9cf70138d6858ea03bbc37b8426b3a2ddf0448679807f87c9ac142b39bc8fe4b8528cfd9186d60db81f3b4baf8d138ddf658541be83a5b8c1517e179ab37dcad" },
                { "cy", "5ae49ef56ded216abc5d6ac3314a695ea46134c2b23d172c43fc33fc27c556a8d53aad74aac004ff878d6f0f71a700dc8435254df000affb6fa3a23a984fdabd" },
                { "da", "c2e36dc7b7958ce1256a9b402c1856d40cc91f13be3b640be8f6b21fc56cc67c686a44958c0995919d4a8d8789e6920d26a78e62dadbb29839c92b3bdb4eae31" },
                { "de", "4fb9fced6289e44f25a0dc7ecd16e121bd6993a611352730e628915a13f61c72ca68d63d1cbe98f91981963a9d6c9b7174d4d71c92540758e8b1b4d5c7c76370" },
                { "dsb", "3c1b024ab8fe9b7e5333f04985e2f57549bd922e4d400ea6465b827db41a62be75c2ade752acd53e8ed3977bad83155df980c1b5a96ece3a4afab195a69ba1c8" },
                { "el", "c846736c3c40d6e62ca838247bb91a58a03413e3afadefdb7a4b83fb7926de465f185816f02340ee62d1f085d9191bfc7154e3db374ac8babc771322990f1f4f" },
                { "en-CA", "82bdd6c357ab984991aef10098cf55ad0568bf38b1f28adf787903b83bee72f1c84ee9cdaaa8dd12ac2198e7910b4ad30ec69fb9b3b48ab9d4d369e5f0abb917" },
                { "en-GB", "996b8f915ba69324dc64d8a3cccae420ec88841e1bf11cb0241f1edc1cd0c061a82b67f0fada4196b22e5331e5659ccbbece3e4e527dc525fd3053d7228a83eb" },
                { "en-US", "2409eeb0b86d69164ba0ff09eb6eeb764cd8004aec4036a9e194ad4f3fcd860f33fa3afb72a533d03bf46c41e6ab7f6f96e177649d6890005b7c96d68e998cf8" },
                { "eo", "c7bc3aef23493c7e07aec4239a088cc483ebea97550523f37925c6078fd3e64449fe49b719f604a786eec886764a1a20fe8c54c3fddfed9bfe09dd202fffaf50" },
                { "es-AR", "98033c0db733d61304e68de32fa2ec05a51a9b11af1e0136996f31e80161324af9782c1943a80f6ac7bef961c6c4347712af6f9bf51c7e768d26b83146b0ef97" },
                { "es-CL", "af64426a6f86761f7c259527d403e9343b7c483498b3f262a15e31e23c15208989723747d393f922c1e0f6c9723dd37b75f835d7a43cd38de5afd313efcb4c6f" },
                { "es-ES", "db91edca0e1cdb262c7c0b8157f34e3b463c086b8428fdeefe413e597f9526f792ab5697d91c6990866cc55380454d5543a266f8d31f9d5dabfa0775718f046d" },
                { "es-MX", "87c7aa34c24231527344f3dd73eacb541e99daf56730c4d97b507575f00ce05bc4ce5af908b86988a37d3b58fa578533eb25c2e7c0e98cb3faac25f6b2778152" },
                { "et", "799a637ea8894cd9c14486ebf830792885f81174a37aa4e25d6e282b6c57d0bad5281d76018c1ae79df16e3283cf10db497c8cc25e997f442691cfa1070ba6f2" },
                { "eu", "e0f5165562ffaeea484dd30f17ae650d6c60e8dd49bfa2bfcdc3eacd480c5d286ae01913d6554eb36d5d030095a8302cf8bdca9a7e7e9e43f140b6edf169676f" },
                { "fa", "c475f3ef0002977f9e39f042cb43b22ed46b81df93fbc163e9161c4ec2e7911ebba5b71f37545f19816311a155cc8c297e264171f912cfcad58c7fbd43fde78a" },
                { "ff", "2827069b60e9824e4379ad724462fece019415572aa1e790a2a493584396d97e3ecb157e773038b47088bb452e00d43925a6cb872c441d66792d4723600e8e72" },
                { "fi", "a112b6174a6ba09f40d4c1502bb7d938fef40e11a100d747823e5f32a106572f46a6fc884dbeaa303db1e72818fa3aff6f064dc89ed29519547ea68c501a348f" },
                { "fr", "504c302b195d880bf4b544d5645110cc852ee0cc55f645f6a3ae3d3ce831d8076ef2f588dd88abd6c323fb7506a3160ca68be83390f4ce00d784ea26656dfb2b" },
                { "fy-NL", "2598ce5f71a7ee6e9552ac40abf3850582883f37e6b700309d66daaba7dec7dcd69bbd7fa70588ddff3685155b45d4642e80110765bf8b74ef518737df7bce37" },
                { "ga-IE", "cd58b7ba7d3691601e6052c964834bdeb685c54f8b31a1d4295a12abab972440e4b7a4460b020ca987e33ad29c1dbd7f0ee8c00988389b9eac319118e628b941" },
                { "gd", "72c635964ff9e650b513e513f7327fbc5cac16441b0dfb10ad52da980e34d2af1985f40e7c1d7fc1661d8f2f5906f451b956bd4cbed9c8b5ae1450e9a6946a24" },
                { "gl", "1c427f09d29e6783cae889c4c52002348a3e044186f0a096b87d805889eabaeb969e568d87a7db476b15cd1b7883cd906ff2d9289dee97b4acaaf219250c16e0" },
                { "gn", "f947b99e2f52957304e938dcab907774aee1c8e414ae77b2b6098e0c6a8384a023de5cdd20d981e63fcc5a930b944062b094fb4640e8517d344fd2666ba3dc39" },
                { "gu-IN", "e85d05b4339f20cd8f63e51d2891fed1bafa73c56e0e33f9f46d33ac5c6a22123713da6097a482585fe7d8086f1872974fec1027733fa167b0006375d88dbb47" },
                { "he", "5af4f98914b45dc7c195ed79ea2023ccbd6f3f9330c13d0eb4e51a5be637cdd8ab311032d09111c3740cd6997b1dc20bbca98d7956e898f28a3bca9f8670275a" },
                { "hi-IN", "04cdc7c919c7db2d7bd14f0a1b9f6f66ea81a9db003166a93c9670f06829bb620f28af2f78045bf6cae950b7a342bd58f544c05a967c2a92ba2d45ef64981348" },
                { "hr", "2bc672e1793bd6ddaf602f52f93ecf1c949ba38671b1d305a230dc09a1e6f76e72e2f0e32301721d6db653725df1e8a3dc1ed5377976782839ad657a1e197ba7" },
                { "hsb", "941cbaf52841c46999dcccc992b208a1e89f31cb1d945d108618ec7f12a747c526c8242fbcadadfacdaa484e147533baaea808c800fe9c6ab1e3584f6dfc3150" },
                { "hu", "805e4eeb36e49d67cdee2246172f96f2272bc3777fc7b867ea0aef9da93e75b5e3ebf76d543ea292e9f2741b0cbc4ca79f1ffe62fdec91d3cdeea2b0fc3f1f69" },
                { "hy-AM", "bf9fd29e1e329f48a1097f0ee68a593ada36214e3e1667bb74463c988632cb40f389a52503debec8f7978d20ac81ba925a6d60a54de030a022812df812ad10e2" },
                { "ia", "4d669c1f288b7d1a3e68861a6a5d5aa95ee1405c8dfff935c5c14de17052f35730cea966770ae538eadf2faea5c7589f2f22f657404acf0cb70ed5035edab7b1" },
                { "id", "26be8a65f8b2bd53ff857d20ed12fbceeb1911ddf874bb569a59c39cf228676ff3b4167abcca600bb4cfe92504bb09d66b3f9dfebcbfa9c697f4e10cf0ec2567" },
                { "is", "a5b61f9ec3d176a310492685df91d085e94007d8d199ca77cb27724f64aeae1f49171c5e6dc5e47ad41e43138907f46ebfb42a2634233fb0daedfe99199805ad" },
                { "it", "578e055c55834a77579f5c2858beec4438a458df7b67c898b7091160460d1a9019fa827f3cff13882a8742a3db4f5925a65ba6e9527c48c22e8c0a78ba2e02f5" },
                { "ja", "f4a5924460949c9ec895fbc71ea4632637e60364e33f10e7406f97e12e542ab0685944745dac07b77693384040ee101f709d9305526b93f71643cb477caa7363" },
                { "ka", "d5412e6bd674c3bf8bdcd20eb64298288886c5d4d0740932a8a7e8499df9e4005e8594c7ab3ed6f6e5652c1e1a9581dbc3d5f4ac0e4dfd24c4987438232ef8d8" },
                { "kab", "24f7ab720b07b6c0d569c3269627bd3ec4c2860e344b97a3c3f1a1ee48d98606431a732c4dc14cb5c15ee14d42562a473a25c5ce2626701886e3aa6fd2318664" },
                { "kk", "68f1367899f8406ab891267efa926a5afc3cb644747e8be3d3589be2baa766fccaa35e41b8e0c465f2a66de8c594106ab454a56beadb683bee65cf92d2e9b1ce" },
                { "km", "74f5ed646ab5960fb6f2a1cabfcc5f85da99c8f285aefe4d2335dda6538d5fc6a10b9e734997e59c5e2e88faab0dacc7eb83380f0df5998d3a403c3a67464005" },
                { "kn", "2e585a0224944c74d13365f9a2e6b7de8d45cc1cf4d834e4220d2d39ae524f74fac2fcc9f539c61e235d357a2364cb04247bbfbd200d5513fa9d6826813b26d4" },
                { "ko", "dca5aba4389a6df60cdb4847656a51428619376a449668519d36be21fabb5b27a132dde77ccc2b34b5afb02a7a0169f7189127ff925ffd37fd7c5d61bf0c8814" },
                { "lij", "3bc08723369903c50f254e1240c4db82d61049e8515a0317403394d121cbd572180e1d35d2dde4e53712baa7f335be5578ab57500ecba1107dddefc70d627b55" },
                { "lt", "1f1cd832d92c1773b6d21fc9a40177a90dd670802d53664d7db68baf492237a9f13d6db45c44a58eb6df1b1378684e64be04882cc23af6f43a9b4b3f236d57c2" },
                { "lv", "aa9e16ff0d2adced0dca8070beb34112390d943a9db7d4327db6e1b0695a3460f26204978ca69464b3e05fb6835bea3b4f9328715460360a22c9f3db6619a0c4" },
                { "mk", "79cd17ca508b9aafd341330b2911c1c619158be19687f191ad40bc3cff50da02892b76048c8d1a7322d659a788b7593caa873e283afb2590f37f438b37da9b4d" },
                { "mr", "7658ae76c327553aa12557c1319db1c60354113e2d7bba8ee30b374a5c8bf397dd9af444fb08da85328dc5745b682b3f43724fbe91f5d9c2b7dac396807e7416" },
                { "ms", "2ac217e33ce844b3326ecebf63097f55ce01ab8d9882d1e3bf22ce017324650ce48945bebc65f54721aeb216232bc8b8c2d85dca284798da23ae605589c1cd3e" },
                { "my", "ca25277966f325d5c97f76b8df9f3d8329b6e3a809867309b8d7528699e2a7ac978ebb6a01513ae3c9a3e2ca86a8a270ecb1820710a09e503e3de67bd40f1b0d" },
                { "nb-NO", "4e701e4defd1dbf6e7b232e5195e825827006b1901e78f95b220da28f81a83618bb75f5f7df67a29943eaadc1b086ef03b2f51f57ea029bc792acf2a24e354ed" },
                { "ne-NP", "0e5d6f91078030ab77bdf040ae6115da9253dd811a9e71d96aa57271e2e880e2ac3ced2921e6c03f3b2a52347b663c55652e13a435477caab9673825afc82d76" },
                { "nl", "0acb51ea4c39d6173d9cacd9f5bac8e2afdb3e999406df2fecd1926336665f84037597ebb06efe1cd8565af4df1993f35067b9a9d63c70409131c3dbcda90124" },
                { "nn-NO", "ad2586e2e332971644993a378d4f06fe585180bea9e4b4cb2f9c1a2831819f487fea01384bdaf9b3bce3e851b02703bc78b23f6ec5c88f871730555aca0f141c" },
                { "oc", "1b7b65ccafd9093fa0362bab707070f5c627f5738144ec0f0a38da3b15da3269316f5474e560a23e8811a387afc8eff3b5c9056711a272fdcc6f43a7aca6d12b" },
                { "pa-IN", "c47af36b8ce607258d002763cf81506268c7ef00b0a0a2bba95e122082c54826cd8b282d4b45d310a3526fcd9ec565b9c115e3c5607f7aa4ea78616e51825c11" },
                { "pl", "51da15c16b645f316c79f4962f83a4a25072b930a907ac28bbe6d0fa852073e84bf8a00cdad03261947e94a166b559d9581a831a72030f7a7d1a685e434607d7" },
                { "pt-BR", "b841b252c6423c22167be434cf29fa0f47fbe3f30f4280a8b4375fcac68d3dbbaa1dfbd2712b55018934696795e20f15aa02e0799f4ad06e2179b7538c0b2fae" },
                { "pt-PT", "8eb3fc221981402797d13ec1a1589c486c34bc93c6376c8716e8f925c61243415609458ad98e0e6259b4659741095d4d56327f68c2d7fdfd6e0db4f931fa9c70" },
                { "rm", "f11b18677b995c8961124c9e378d1c9131014ead9796eb56887076df339785e8a42c908aeb7b8e3b829530d70929569d47ecc5a7b5ad609a2305b061f652a04f" },
                { "ro", "9fd27577dfc087d335aa930887b3558d03340b969c42fcb94130c328f8d00b2392a609a7c9927791b1a5784b0649f2e6b59b2c6e17a162a276305dbc010afedb" },
                { "ru", "e779ae28dd4a3f7982b484dc7b1ec8f7c74fe5566cecf6197ab8b366374549c4c121f0406dc3032a71dfb8fdcaed36bb5ebb53b897d9cdecd57f2df3dfcaf60b" },
                { "sco", "79cac3c8bda2137d35037b5c518ce5ea43fdadb42f83777c7284ac2add8b490c47570ad33b77a7f11dae78779d496b134a6cc9ab1dc1e955c0f30e13869865b9" },
                { "si", "77dcd7bbe7210810c388a36e0321a55b0c286d0e8d333c38184f208cefb663b19c7c561585f70d020acdf4e553b632ca190b4311861f223358c320b7c2fccd67" },
                { "sk", "92988d117d8ee06c6391bd6a603cea37b786a50c40892408fc6ffbdf0fccc10bc91bef20d201fcd929a64dd42ee523bdf994c9a52e0a18f44b75e0ec0de382bd" },
                { "sl", "70dbab22402133d3e5e9784ffc666d6a16177b73df1c147a48dd41637369a703b13b54d3e5b50a924fd9da720b9e8d8924777205e3c2c490ec9758cd3f3d600c" },
                { "son", "06ea609168a30ce0e0f26f648e59f3022069aaabced6c5db58671028ce3bd9403d1aa8b785d5daf5b03cc07a64a16b078c319810c11e600cbbb469605052c7f9" },
                { "sq", "b925a498f74ea583364d6987d58679e0506ab7c437945ba18a9f0b405b1dc12cd14bdd7b4ff7ac953a77718b26d7153f112984dec3a81f248a4865c5d43396a3" },
                { "sr", "1278c7aeb027063d716435a16dc3b204e1fe77bf626083a07abdc2ba2cdbb7ca430ebdaeaf293b2eb631064f3ebcdd2b1090c5748ca1c854c1f6ea7f72721dac" },
                { "sv-SE", "7a5cff9d7a2041ace61467c1dd0abbce65ad1da043c55de24e6ee0647345511dd0b2ea69506bad4d650ecd9ad85d9b709692d8dd3d1bb963fb2fbece85e05a10" },
                { "szl", "e32099586013d7217024fd8956e192c8dc495c714d607b030da8f15161a9437378c0aefbcc582827c36ca5f86fb0840b4c0eb97109268461b620f2e2f175f339" },
                { "ta", "a890cf9f769e0c975e9d70d646aff883b9964a68e6720e0dabc663360cbf73602b61a4452a04b2d59365dca412b32a58857bf008addee767eb18b37a75c8b316" },
                { "te", "03c3d1342692da3af2ddfee663192d3711a1714aa4bca5acdfa0446dd05b2f7257acde5d8473e70a6d125175583253915e8c3563e59c5dfaa3430c2561d0f28c" },
                { "th", "e5fa37dbf0dc3169885a3aa7abf1abd34d0095cfaad0abf3467b0bf1b8f415a6099d732d251873cfb2be648bb88883f2101867ade687715de985a2fe0d7d1532" },
                { "tl", "a0178f3cf4eb5c4a6cdc79ea935f0066afcb6301f87bb72149152e4c95bbd1b4c4f19acd6d9fae1927fd2d79e8930e571f1c75f62d5dafc979bd59313dd9d829" },
                { "tr", "285fc291d72a0ddd9c50af46380e04699868adccbdd5156bc8e22999f272b60ef52432551f05b37e15f1bdf545969aca59df87afce29ad40b76b2ef045ad1538" },
                { "trs", "794e7ee9fb516d5b46ca28b13ae94a8685fc69cb35b9802e8bf87368e52b7749835316acaac920bfc0c61f8f22ebf496aea22f3ec57db2c9279fd4792174fd55" },
                { "uk", "e7eafa19aa766e4ebd16adc2ff55a3c8b23c50556cdc8027759693148537a8c58237a563e3086a545e2636c9162a7dbef102d9f5066b380cbdf736c698fa9b88" },
                { "ur", "fe87d99ef4d2ac3c34f560031626b5c1b140b1772e8c2fa1474cf698ed77bda17edd88415688825b4c73ef0318798df226c39eb74e37c4ca71a01b2ea32e6e08" },
                { "uz", "968dc089d2f6f2d5ba439981786ee6d2a10676ea63f62316f1e78ad03b66669e873822156e58fe933e6378af77f739c39947e8732c91d94f8b51141a90b27852" },
                { "vi", "fa2541cc41fb482ad008ca3be9223c26e7e85e8fbe8e58b8e2b33019a681239ff8e41872aa629a093ab82f30fd00514e7dc366079738fae1984b5ac9cb9a444c" },
                { "xh", "81d9568e0bbaa0493c550cfa658e62db8103c83c677b407901b92646ee696a58c7e823e3122af6d51862adfd0f1284378999aae25780322c5186ce762ca4152f" },
                { "zh-CN", "7212a79cce2d7679f8d63205270239cfe5198793382092855db8f5dd9b6ad58579b782667a8f09d2af3c751ae705113dc9213d578f3e869d4d4a45afdec33b97" },
                { "zh-TW", "1d17d1a9a215a1de1b1a47db8d534062733103acf0f47bcf80df8f8e8d5f7aabf97665bf8b4aa9918d52674cd77c9ad0e2d532f5a792df1becb965b15dd38a51" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/109.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "2f96539ae94be6e8c3cbaec0cebb7a97017704f9b1e3ffc3a91d5a6c3dba23f9c46eedd637d834dead1301055929973c120ba3d51841132827ee873f9cdb0c20" },
                { "af", "1bcbbda1bb806d4cabb6e68a0c6883a4a4fa177c39840f923f78a76e5e4b893fe5c2f5e7f5d021cc799600317fee2bd107f9e16fb7eff8e1ab7ae718ca4736a8" },
                { "an", "4a226346549531edc8ab4bff65c2d0a790229c1d2bc677eebeb16ee3c7fb9392c48c77ac6a9f89a7ab536b0577af87a3e1c19a7cdf1ad52cb490d45f141363f7" },
                { "ar", "a4789284adf354fe14fb3c9a75c275e4ebe3f15173fd3848971d0056bd50b2990c385800e48b2fa46c8ca54016f84e4bb547b5359584d3b79b042be49500e055" },
                { "ast", "f4ca81bed848b9150166755a40fc0bacb57d2bad3cb2a47e91501c6ef3a6f10125287ae4abb8e51aaf4c53145824071a5f05d6f78bc3a8204c9a70f2bfa5d229" },
                { "az", "065466778dca97cd324a9a1cfbf3999449c0f59639c1943ae241d5061bab9a343bc992a8e461bbf03ad5ba7727fdda06189c99e45fdd86734ca3f809c5d31553" },
                { "be", "6a82d1315a723847771a2c047d661da8c3eaf18dd7a7c9d40efaa1c10a54f623b0f47ef652021217e3be2c9f98ff1158bbbc4d69ac256f8ae56a0fc8d8a24a8d" },
                { "bg", "e1956358c60dd656843db103ca736301516324acfa452b4e73fac116d25863787550ff7852ccdeb11d1f12c2d81e9543cbf9fa859885c9bcacb6c1c41bdd1f0d" },
                { "bn", "1cb112106d027be2df8d8ccaff18d8a15ebc877c59080f4e8af5daef07359a9bbf5723901e157db2dcdf08ef51fca29bb395f370cfa523c6a0b66e467967c3c1" },
                { "br", "6782a4159bda37d748f4d5a80aba4f86dec3fbf1cdfd524635636f06c60a57ef9ba9db9390484a579e5758c4f7e4b77e620469833fa2833f1217fab39e7a2d1d" },
                { "bs", "361c061a3dfb3730b0d7ca3444635a121c6407043fa16f37c3bed4a3119d85eadb40f77405804024de499f0783e767856226455a29a85af245816fe0e43db2b2" },
                { "ca", "53fb38cb35e8be89b3c9b8716cd027ae9ae083b0dd1c5bcd83442a1f61447fd8679c1b5a0e6c838c1e46d5eb74debb5e34d0c3556ab526686a307d2a31bbad35" },
                { "cak", "9a26d7fa5b7fd7679b118948f5e3661ad52e93552da5adb5b8c86c2aa4cb22c3fd6560bcf075b55e95247c4c2316d7c78ac0452c698908d27f07c08abb127058" },
                { "cs", "185c0f0ea9f606f86eb06f2f69b417dca2465b335060afcc464a233a0c34ae3b0005a6c62e6cb6ad19d6682b929f4e9b8245a4f5c3b89553a4196324d4c8e59b" },
                { "cy", "55c826f5599355a895f28b98bed88f1cd34ca6fc59472745f5b17ea68c97c946973ec46db63028340875a247d5305de9a90c7555106e0a5c3a2c92bbf1bd244e" },
                { "da", "49d18467f7f234aa781ad59cd9789e62eca0181dcb147c49a7c9077486a1f5e7d29c97a64e99dbbd67bfb82678a5915d3f4a4530c45bc6a7d8df9dc671707d84" },
                { "de", "5077a4c78d668fb9275ebbc07b98dc2346f44345c02187dcd53765f1752e2e707dbef0a978b02b80882cbe2c9df11a90f334cce423a176598d6cb63f990f8779" },
                { "dsb", "36d4831c4aa3a4b0bcc27b4ef7e9829fea1e915e48ee93989c6b8c370ca7e20019f2547bc0b344608d48cab27af8280025391d21c1aafa259878b80fb55e1d45" },
                { "el", "efc749bd3e0a466a4f203425515d3f558ba5cdfbeeb6bb2e70cfd175c0f36aa5e9e289cb69e78f8614e234416da3e4531144b305793008b731578c1a19f532b1" },
                { "en-CA", "fdde7e1cea58322b89ce17f269eeaded4a783b85b56fb0cb0b80984dbed8f2f6be86eb3c1e97a97b6873633cb08ada042cacf426d840ddfa3fdf0576e9a2029a" },
                { "en-GB", "3e8e1616eb24fd383844d1699675de8c77dd1d54842a29f3d5f1b317296c2ec7aeb28f6b1ad7bd48c839dfd3503cca6310e8e8eaa007db7dc8002c35041a3139" },
                { "en-US", "2b48239c14bb687d44ac56314e481c00d494047705bb5ef4e6ffa90e86bc07fbf156c79f8aa959c22815340f5ef595447ea3a6975f645220276d1195b96cd07b" },
                { "eo", "aafca379bfec819a39518e5dcc39e24651b5a72dc4918b2fb88ffb3e3edc0d61c83834aebb3122c619c0394f4d7174c9b7c46dfed9c5603f73a9e73c70d757fa" },
                { "es-AR", "38fc846d6a9a589bb204f43d50177aa321ee011e585dbce1b2e3ac1223a4bf417a169be86f02e360b7014de2098228050735f3976ad6f2e6cf855d301f6cf35f" },
                { "es-CL", "e8f4f7fb4946a770a6f974a4fd3dcc09da2da936659d733132c90456dd8a65bf11514306d2605fcfac199fccb148f7b6a06a41feab09ed0d2468fcb97537d522" },
                { "es-ES", "67244402e598bae11546cd29e6bd8a973d3a08a682e1e701a7032770d0327208fbc650998b7a83b4c9fcfa3b926701873fbb9fbfdac1de2a9012ed7e8485b5c3" },
                { "es-MX", "c75aa73d8feb9f0eea58dd3fa4e0562219bd844683fa8c7b0e24729a9766c846747bc0a93b2042d96bf47407da190b2d18aaaa5bc808bfb02c77e2cc9205039e" },
                { "et", "22549c397918ebdb83c68806dfc833994ba988e15681cc4612af6fcf8eb88dba664b8a88db2fbf8f706f7b24ab3b5a94cb0873fda18615dd511fa0fb03c79785" },
                { "eu", "d706e4a43f03b5a13d22bc092bc2900c739d7aa1a42622c574df5d6b0f2702d869c82d12875fc3df10b11889db43b2abe918d2ebbd6cfc78a0e89ced235389ff" },
                { "fa", "ca2cd626a6159f0b5517c879bdf7aff058ee3ec321760361b41f123328180435369557e1f71b7cd4a7c09a20f7f90acae9a6d3753157f2e725801bf20a0578aa" },
                { "ff", "20022a9e488a959bbb3683ff0e8bfde3197a87d25b1becd54ff65d5eb584ea369e8fb81cf679fb4b7ca4d3db3929173f22c6dd711d5554a03206c9dbbbad6355" },
                { "fi", "40c24aec042e81bf609d3ba47322eb565825562a7ed6973d3bc118aeb2b99840dde18f34b23e9994a59a847058846990b33cb9aec50c66a21b1a6a68832856f4" },
                { "fr", "29aa49e37bbe618e90c4d4314ab0475e525df28e757d54e36b6d487fba119779c1bf5273799a1dfe63fc0ff0be732ef69d1a5a092c47b2bdd9bf72d0c94181d5" },
                { "fy-NL", "e4a58499fd57740fb80fd98989179b6d02c7fcac153fe967ff776e6cb501cfc6ae80638b4cba915848586aaba82dcfb4e6f5543fe8b30cbe9dfa615d8228e364" },
                { "ga-IE", "1acd20d8677284236cb618322c33d2619cb3e85576773ceca8c9698f4becb1c742e395ec1610c50cda34d4f80a3ed062bdd2e55a0e75e738670019b442aaacce" },
                { "gd", "9de575bb9111244e257704875224b3fdfa06420191b77adcd6943e4f3f74e0acd24454e01577f592bb61a1fccaa48d44a09f6d27537c9d06c107a7f2063e8758" },
                { "gl", "2aa88990d592c067ecb4726ae70b06ceac7d59ebe751cfa4dd1dff936cfd8c76664df199589a7ec34de1f3495588ab3a81367e4e47b77e35bca4d1104f060de1" },
                { "gn", "6b0b0e6904803b714b00784bbf3254c83043e854773880a051ad901a39b8292ce72eb49591fac67041c40bbd6a14e3312a1da473848ab86c8adf2a0525ba4519" },
                { "gu-IN", "fba1afbc16c8b9e0c1518ce4f6c43f747c798e016a09c73526b2b0b98fde3809120721f5659fafa90b0933ee4b7fef36fc1c863e69a70736d4f198fe5b454667" },
                { "he", "220a0706aa23a2598a85f5bd8ad26f71ef14bbea028d975abb4274a7fe50bd2a4f1ce2febfb77ae19b722630ac34b447032d133a57b432a347d1e310cb3ae81a" },
                { "hi-IN", "7f8195c819a08b931a186f3352540cdc3739f99827cc648f3785abc91b388eb917f279dd642cbc1524e25c1eab9f00ed01dd3b2b7084af6b0a2e5dd437c343bf" },
                { "hr", "c2f729a4c199d5ad86baf9e0d761d86e24a33add5ec8842a9b817eb56431a3a56b13377d546d5e655b5e7fb9545cca9ed5d05207220fa4f0a69aeff58d3a086a" },
                { "hsb", "889233f935c43e640d7260e72a6bbbfc2eb0d5777f1bf0fcad666f229122854b2d700eea684aace3c0e785e7d64d18ac9e9f4cca03b04823a3cb58cddc4eb124" },
                { "hu", "a3e8a392ff8fc7c7e493ab53d2c454f7c8aa4795d4983302ff78da482693aa5750f1fa35f6b17a942df85e4e67770f45ad1def2245a43b49f83374db8e7805b5" },
                { "hy-AM", "55e6eb5cb76dff637962ee33f84f74799374b6457ac66f16ad3962b8220c1c9fa558d0985abbbcb10597674777d9c26338f6ddd5c6d72d331d787af5938fc88f" },
                { "ia", "a0a0dcc26596adc228c8745865d2042516c44132f2a93ce09ed981830894b580b6bcbb01febe55da9ecba41631d68940276d7c103d92b0094a155592148bbc00" },
                { "id", "02899ce407a7f52c3bfbe2436be163dce269acbd5123e71d75e0b4259d77e47d9858f4343e94d383f724d7f6b57dd4e1d750680302a16196498f9b0a3d520fb3" },
                { "is", "f305564a227db782e2db506c2d6ece60a8284d8128f4fc1897a232a920564c2495ddedd911c7bad1f1e9e040e4e77484b0d416271291cb5901362c5b3655ee18" },
                { "it", "c9a6c130c0b56c90524cb9873705ec05212f334212a850bfdf37bde01d11ca28976326686ae970dd8e4b5b41d070a9c7506642e4adc7b117e41993fa542e09ea" },
                { "ja", "8c4ead8f95ef590835fabaac2e4be68f2f1a987d83bb81a21852f13dbaf32964db5aa7b1481889d19f89efd86da87f24aeb52333c35f5f730546453ee163f033" },
                { "ka", "ccf5dee9a3247df641c058ca7d755842037fb301553fab74ee71f2083674447cb857e277102b91d278cc842fd5ce077bf90fc81f8dac5172000877288db837d7" },
                { "kab", "b5c8d7660adea4e6cd9ab7b89832b0ba3cee194abccb1fca25b20418be506ab29f87f1ecc91e15daec0bdbeef66da34e8d2803a9394da660255af6fba4ef0cef" },
                { "kk", "e92e1a556b7e02cdc46227b6db7dc6f9ad30256dec8d04ff1ce9a00709a7eae7db343f436349797c0f53d7a73b7254b7706599dc0aa6f2b0f3a1effea4acc2fb" },
                { "km", "f13cfa05d7cd567b1ccb35eff2f1985a499f2da6f283286ce8579161cb75624a39db14b9fe84594d3e5216d9a37ace094c021cc6f6aec30a6742cb84f2c0d141" },
                { "kn", "1bbc5ac7d6b41d2e33f3b5ef7db450ac14848246b69a096d220111afc554a1716cc30320af4b169990e4a000599a7b4cf8d39b8033908ac26e2dc73d545e23c2" },
                { "ko", "977ce73798c20fd26dab110b5d2fbf63c20a4914924b15322502146dff654ff9fedc8914fba660d682a77f9505b9cb01359969f4f1e4de68147728286ca0f530" },
                { "lij", "79020d97cf7b815da91b2546b603ebdbc42ca98a301918e12fbfa474d7c85597626005ba6e60bdf45b8560c2e0f77250795ff31e7d2aa1f60047afe0c9ef3a8c" },
                { "lt", "f34329213582e81ff7cd8a5c191fb5739fc3c5027b013b699f8c3edafdb062259dadb4905c515402dce260e0e01ee10a90d7b78d69f6f5bbd883de8bf93ccaa9" },
                { "lv", "f691e3c330d0a245b3acaeed0df58ae265c3e53cbd4073ad5b336a3371b6889cbb1c12d3af879d3b3148ee3702fdda33d63979e6ecdb20417ae62e5b47bca076" },
                { "mk", "95b64a86482f0c26329f074dd6347a6bd3239e7fa65de51ef2f364ee051241341441294be1c7d00f4473444e7c042d67529fe61a6cab51957e5d7dfc7fc93491" },
                { "mr", "947eff15dcdfdc4a344119364ead35b9893c0f3bcb358a8fd828616fefacc036e2dcf7c6294cd1248b5133c0da26417d2f3d41a53b472bc81f64ee73a758bd2d" },
                { "ms", "9b5a27f76e5c8daef81f1acd2f599b2304361684bccc643c3523fa06f0df3b37863ef415d3264a61ac9184165b9a2a3b0d8c5cee89f4222def992c41aa90defe" },
                { "my", "01a54f4f2125062fe4d87d1b35fc68a34d66960296b1c4a5cc663dfaf10bca2c800306731d94ad68130847c3fbd04c65306e316aefac71f51ba65e5875b611ab" },
                { "nb-NO", "de790cc3c6ae0292ea60c9cdee91c61c9f00359f2c0733dde4917173bdbe004a0ef5069f2c7ea6ccedb5d419fcce874b08fcb7a46022640b37eecc4db4bdcfd6" },
                { "ne-NP", "d44f308b939f5a8ecbf5fb4e9f0197953bf079c00403c34c9fc3fc7e514bb0d585ff2623c69547a300e69f585409d712311ddff25373829236f794038f5b5f48" },
                { "nl", "55ec33be78d84032140ed198634293110f4dbe9d1bb4fad6f7c425a4443bdd22c07de50cae0ac135ecfe36b602fd89dcfa1bdcd0ec0b78fc3194bb9b601ef55b" },
                { "nn-NO", "365836b16688d5aab4147afc2a821a2d155f369f3511186a76830884984efb12d9651b30752ec38acc71a44ed46bd40bb7dd40be24a3f53b7d7be9821db16b03" },
                { "oc", "b6f41e69bf8fd49d5f62858253ee940fc4ae803161baf93e37719f12303f67aa285d4a7ef6bb7276eb67166434ddb4034a6b82465643ccafd2e48be182216075" },
                { "pa-IN", "17746d181f7652b5aace3e9f300a7c259a28137e6ebc97f68f404af8a56da4641941d3ef7eb7a2692c59ed0bc1b5984092ba100c6dc81e6611d7c4ab9543c530" },
                { "pl", "9f273b2947f0429edbec7ceb09da35d5f594742e9aecfa8b1da004bb73d681bc9e17526cc6c917150c7b0e083cf7088f92d384b25f7de67f55893499fa7b92f1" },
                { "pt-BR", "65bb12b5ddf1afe2c1ddd60014fa8a0d434cd476d637afa11064451e44a68df5fdfb2104e67fb2d80540dcdcf85afadc8f9b2157072e5fc9cbc33941d5ff69b5" },
                { "pt-PT", "a0f10405f78ebb468d26140abd4e2051b94b009f3ee5bf2ef6a4f50cd1aac10569b709548ed0446c1ce80240c8c9c7053fbe0beeefb1b3377887740074127c1f" },
                { "rm", "d98fb165c7a74d87cc0952647436f298eeb8f8c9a12bdb9698399caaf8eb5475fed61960d73af7ad6e03cf3fd4d02deffc2c90415c9fc0252cb59a2b84b303ac" },
                { "ro", "0bca2d03fac6207caccd2fc30b857efb1faf0c3743dc92c74bbb4cabc8b2b0f9c29c9dde31e8253d51de9694d92ef368ce47c53519152dc698f3182b69ccf96b" },
                { "ru", "3af9aa12a78207a9ad9d4586de87b8218e878fc5c829ebd61582c5cf457c2e6b87b467599db4f007f13f6d106139e27105c30633a82a718643f557ef1c1bc0ea" },
                { "sco", "2f185f98ed6a4495aaf6c6b827002ae41d0160c71f0b804ba786a9b4a7b7dd7fe96cbd01df78a07bfe276660758ae7557f2af76862ac7a306a0083fe727105fc" },
                { "si", "ecdd8bfd06db55c53d2adf5c2bc6aae72e924d677eacba9c0e6738484871d74c5caa8f4aef9c1927ae9f795792b1d1e5be480f3207e0c49b744c0545ef4c751a" },
                { "sk", "043991ba6b6922a76a8a0448d9a68f15d0557b47198e25beed4f1972e488a4ef40952b14932eedb7818260f772930dc47a28ac1bbcfcca2d43483df9df1ada7e" },
                { "sl", "c9400711a66ac7e24dcd0c0d82fd8b2127e96433907fa8ffc6849a98a63475f2dd89f13c77fb036635dcd5f0dcf38ceba1fd7992462a53184ca4d8807a3ec7e5" },
                { "son", "778746eed04bb6c2b58318c58f7a4f7e9e9454372af575f6f0e405a3c5982766a591c91cb78f92810673aab0539b779369b8172e1851576f8bf2ac4e444efa2e" },
                { "sq", "a2cdf7bc827b74e54a76c86f6f1e75ad72f520815a1b8224fa251888b77d12646a20c93476a0128c5dd989daa5164d7d01e73a6eeea20dd876ca9d4b4e725280" },
                { "sr", "ba3efd47625ff96acc22b257a4a2fbd9e4cdec40ecd39fdcda771818d191ae25faa52b66e186de06b3eaaf74b518e4d7a5f824a42b0258f38a56188df1216a5d" },
                { "sv-SE", "a8126312c77466e16e59d9f41db2acd4ac8f5bdae0e0d9c5bacb95334641a8770bb31da0a96f22f6e9accbb76fa112f0e38153df756b29e77993572b969c222d" },
                { "szl", "d13d68037a10c0b1cc89a1082a9e3b81214fcc9376ab43bf51113124b3fc2074c23bc2fbd33a5cd17bdaa9a08a025bf2a9369eecda00db9e7cc79464b55c571a" },
                { "ta", "1631a46a7a22f0c86731a750560d27e25dfd6cc9f5360d22db4af1b49b703c455e3f675ad09e49fa22b89eb54444b26405fb769d5b7e759695adc10d94c22ec6" },
                { "te", "a94e2aecb1b29611ad8795c7d434e2af5380fb9b42cc0819f203e4f9627d1c459b19bac6cb70acfef668a2515ce98ac557a3d7986c3b2c33ee38788c6987260f" },
                { "th", "2faeea47d4190b47edbc3e127cb1ce1dc492eb19d58c1cd71007be2f5fa8b470b897f9ce11c7ab44cddd03f0ec6a0b2d396d0e7d555373d840aeb9557b74c1a9" },
                { "tl", "5c2386368ccdb9f60a72db7bbad55cd7c3d77438001aa0a9c95cea521da4754bf876625011adc6f1104ece710941275679096714f069731936291d1dffaf412c" },
                { "tr", "5c18577679e9169eaea249d70188eec92e16fde8d1a40580b9a5437a810b89a8e23e0eb31bddb4210b94ef961acb4db6c0197aeb7d9afe8060a080a37c5f4556" },
                { "trs", "c3278db2fb744c6131b5c60fa13858eea13d985a0331917ee29a84ee6192e2a18f46a301cee6d7626fb30bbc2e7f0fbb20fa528969f4f21a9121f41d5c4eb873" },
                { "uk", "b411fc5ce43ac660b6beef1637c510c2da0a43913477cb22927669044fed82d610e8f0b0969ec5c76303765ee7d690aa0c1aa3fb7d448a9f5dae77de70d822f8" },
                { "ur", "6800dba60c82e797b47c8d9a476c0e8eeacdf51c92c882978f2e678d72e25f8c9d0afe26f4b1fb523bb2ebb6e7b402a7b89afd61a3ce1c3331eeba5a865c5e2b" },
                { "uz", "d4b416f2db7a503aeeb85fcb77ae72e5394a3672865d96109097a4aa2ed3013ef228ffe1b46c196777dcf71c42dcd924adab849bd05ab136f26734191e233905" },
                { "vi", "9d3cc269e29d6ed2bef2de02085b17e0371655d8a1eedef54c09153e906302cb9a9d2bf52082c2fe45b0a13824815a01eb4fb206d7f30034e6c6d199e8a3343b" },
                { "xh", "a5e7ac194fc95f9ed80b8ca6cd6858ddc02ab041e57ccef82c56d1d560f57ff0e2696c65f954c81bf525b9fd9a65809a6c71e93f721067aa95badf0fe47f0792" },
                { "zh-CN", "c4ace26386e640fe52730199a97d22436710a21f1d1883ab3fa53a75d38bbce0e5e93bda79786098e26bc4735d7dd8b4eaa840e7fb539ca49eb207c9d29f06ea" },
                { "zh-TW", "1759c5d02a713d59d7eec1079983a18cd2813c763d19efec1e7ff2fe6513e2ffbf92dba31e929b63c4563a6ebe345310acaf87f80ff7179768c18fbd7e55792f" }
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
            const string knownVersion = "109.0.1";
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
