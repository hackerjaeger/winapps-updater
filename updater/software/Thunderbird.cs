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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8903cb87ef8823778b6e300346107984c144ccc38a41e7c48e94e61eabbba025688527a95500da56d528780012152d5588e5d05a12d20cbf7c9b8f7640fe495d" },
                { "ar", "988c78ed29bf1346c3c7f219b7c0900425ae129844dd4885e5bdbd2a62b89f9f2b6ba4ded9c1e271946c89923254e22491e6665323f23b1446168910bc672043" },
                { "ast", "ca9c01f023e8a0e74fbde2c5476bfd67ab41f35564f815a1df820de8af9bf554a1e46d22deac175653037da96bc9dec03d0dadfcf5630daf65ab61a787ef9044" },
                { "be", "dc60e0af2d13285d95320b9008fbd47ca9e42b6e6286c83f27071e526d9f4f5527e452b64898d1dcd1fbf75c9bf46d7786169fa79859be90ca848356fc9b51ab" },
                { "bg", "8bf91fbf3142341c0a733c0d716f10005dd9ebc08185eeab582e45417636b701ff8d98f15770b9cfffbad8a2566f4fd8f52f76945ce82676d171e5264584f6c3" },
                { "br", "2204b2dd7e30fc8c5907a5eb3263abae657cc67e22e2f274715e5b8f67b69b7c7e82da35f27f658b25fff0ace4a7998e710b3265aa0a411dca6c5b089d412ab0" },
                { "ca", "e8c5aaa483b98e3b2ed906e0bd6f761341a545e58d9f02c1d7dd6f4dc80a7c4a2215161ae13bb99e87820bf6311fef88a9cac9f21eb7612300e494794d28d45d" },
                { "cak", "165ba9025ea184c7408b8df6c4eb2f01f155037a4601c5929b2e243848f399210f9ec4994d63652bf5ce6e0aa3a2918e7ed2e5507bb081b5acf978ed34d247e3" },
                { "cs", "00a11d850f981901814f8ed80b48f8ea2ae29d6d89ee2544c6addf85675d1592a15cfbba0cf5158aa33bb745e3d45704cc75191daf61531154c877a040ca3aab" },
                { "cy", "d2139681d11085d7000ba7d612252fc4f13e0fb25b91539e5559e61231490a76649d442e6e1ef996d4b131d29173cef724f759c6b1c87d643601f4e189f8af10" },
                { "da", "32a6238b7969fb3750a14f600fab35c6359d9b68b7b916fb9226d5620c44016918394523201e92f8b71338690d0b7b76fb8c2ddaa6df966b8483c2656a6d251d" },
                { "de", "4872309103be4aac9c328bf912add45ec3b6665d3bd9b572f5c65ceda65d16fcec5993bfe4c4bfc4e89b4e5c39c14e7c42d8c4fbfe4160ddc1a81b52fc86b1cb" },
                { "dsb", "1dfdbf98944f1dedc1a5869b573e5b5480dca6df7e28112e44c34e14cad5b607df9eed69e3f462c9a4cd08b3207ab5ab6f0080bd8d3c14adc300dc0dfc5ffe76" },
                { "el", "f9122dddee06d7863785fd8da4a8817c7d964ca6217f12465e7be83389800fcfbf6bdbbf714a40b6abc9048f0ccfcad9b3ef7b93294d127df57216ac8f11a879" },
                { "en-CA", "e17c94f468a0df9f13a34cfdd3c31b0e51ac9bbbe67ca45b83fa45f66326cec95086aa3ccc21e1ec3293e6e6c0b26bc36bfde98027441c82904f127b74aa97a6" },
                { "en-GB", "6c73371401f2f7ac32b2654ff4e0124dc079f552030713faecf42771eca6a6d30e433de217261bad7d64d1c6e92569f71d907e5cad8f8d02ba848be85420adb6" },
                { "en-US", "41a19bacdb1fe0b06c008304b1c95c7c2deb067539728400a9c3a935a5e793ec40b42fbb44c547a0efd35af661d48c4d83a445b3f38f5c545a374ca7375e6e0c" },
                { "es-AR", "0e480a3bb8ef5eefaf88058b55225b242a475d5003d91cdc8e18a24d9d44b5ec1acd07af67c3e112010226b00ab40d35d4e51b1df5b06a01d02167e1987813f3" },
                { "es-ES", "9f982082fb7651ff6ab483510ab802022fc9595170188d6250bb71ad740dc6d3c49b01913b2daede43fedf2bf5be6f382461abc3484a5166c279bd5bb825e8eb" },
                { "es-MX", "6813034d7e753a1fefa76b25d3daad7746c6025f01b5f3e85e12ef642bbc465eaec04a53bb5ac8c8b9773008e8e4d8139b767873b94af5e475bc36d9c2b1972d" },
                { "et", "0e30ff993215c9483487978a58f7f66f99c3133fbaef91f0fb7a38598e9ea4854ddd1da1038a072d4a19673ef75617f41a274a019364cc6d44c5ae9087994642" },
                { "eu", "de17bd410868487c2e64b67b263ee135c49a7b2b62d94ecf787cacf9f817f8456903742bff24c378cb37f1c4e657dcf831529453c391e6339988647f92d6eb8d" },
                { "fi", "30345e0e54949f74f59169e1e3cbc8ee7d3f1c65451fd56bf12e0adb316edcaa6ba1080790423e3758e85d32057d5ce7ae7101f044a47c75d8e7b46dba2c57e9" },
                { "fr", "4c6c65829ba6be0187e2930b9b90c3de11da39cbf9ff6e39ca3d2f17fe76e503fafd9be3e6e52b6364b840342f3b8609238c26ce5218c287e9290314811670e3" },
                { "fy-NL", "31179bc521bf6c5634fb187bb6d1a8419e8ba9a66546b9f09dca1792196bf868961fad01a864621d69ec04b5203c79434ec2d1432ebe71b7657b02e64758c261" },
                { "ga-IE", "74a489ba07e941dbf4aaac8afe868c169d1cc05ad54fdd37dcee0c640a70f3b33757a660cca23376251a3fe883c6eab8e6e39dbc8a6a88c8de1d76d0884d6515" },
                { "gd", "0d64eca5a56ca943122fd78ce700b68f25be23899ca769dd2d0ae0092cb7fbdc423e3bfdfdfbe4212a105a9fbd539ae036557beda5e5739917382dfaab00e728" },
                { "gl", "6ad7df0a433e6b0716203a1ef3d95a83fe6bd1f98d15c3f056ecfbd8e7d32d8439c949c36e56bb3e3766ed835e7e8c637764f3ee036e0d7f12b17258f52840b2" },
                { "he", "91b09c615cf5728761cb86190b1ad8dbc039ca582927a67b2a79376e3600c361e31b8f62887bfe157aa967342c139d9702c079c0a907319e36f221e9422c92ed" },
                { "hr", "2dab249c1ee9967b950d00c0011824b19073d4b974eb23d13d1638bf5ceb0b9d1e5d3f8a01ee90559fdb331e3785dfc2be43d852d229efe71dc46ca2fdba2d3b" },
                { "hsb", "d2005528f7dfd9bd2a98dfebc1e0e590500660f6e29fdece1b4fd3a2157aa95cea0eed7156f772767d3aeafa28ab88326dc11f3150482441f12469735f4d9643" },
                { "hu", "bca570fd1e69d730f875f6b1cadaa376d3a5fc6a2deb5cfda6dba40e47d0192b7325a444ed788a38c35f3675d824598a6d1b1081be8aba2a6fc32a6300b5ba98" },
                { "hy-AM", "2f600d561ab8f82e24a2d6187666192f45b95b5496a24d7251cedae648c2d78cf6595b47e7543316b9e8e85d63efca1bda53c6445d4d88d7256d98a872689158" },
                { "id", "2be854eb62655b466e438c2bd9496e501e459a58228bea99444c539d21cdc5cbdd694c5981bcaff6ae5c59639b7df5144c33c150254070da4f6f752997f0fc32" },
                { "is", "12f66e27b13b58f6ef9f729b02a1850750b002822d76b8c3bda28cd3342148b51ed8b664de551b35c1d1c459b6bd35cd560eceae9198adfbcb1fc87fb61166b6" },
                { "it", "674a2094f23f19a7d68f53d7a92c89be13894eb739cf66940d117e3309fbd598a55e615ff26100ea1a613174d8b6c9ac677adaa50c8fabdc04647583f600949b" },
                { "ja", "c5b6562a34f95326ce576720ba72383ff55da21bd8add75845764b733fb411ad0e5fe906ea892cd8ccb2349adc9dfcf0635adadaa7f6ab95479110fb8de326d8" },
                { "ka", "11244fbff05fa5a5a2ea55748381da3f6875568e4399b2936d35f6b4710c70b7e97f7eb6ea2b9f983fc6f4d04bb349655bac19f7b25971698631c90fe1d9a6fb" },
                { "kab", "f56f32a7cf8351b4049b55f6e66ea260d49c4a12bd98e8f6027dcc36de1b2a5eb10a5ff66563000fa6b024e5e5c8a0468a4bfcc76e6d8ba9d11a914d90ac83dc" },
                { "kk", "60875c75a919ac9311b56054b80c2322f6ecf0ff36170212f1cc2f9b68ec46f7172499429c27a61ba0d4fb39a850b4ed614233593e4898d0447a8e7f9096373f" },
                { "ko", "d0185e570ee3b5592b02148140233c5d7f6c986ace3b70bd7397b6ef8c81709a6ec4564c6420d8fdbadc4e1eb6aeaf5e63a7a70ba9a01fc6b33b2787006adba7" },
                { "lt", "4fcdc12d09cfe8657130ffcee6598e46d8749a931e4f53cba23ebcb2375d9ffb421dcaaf3d324d06e161105351842ee347cdde4cb71afee5db3ec7d9210f58e7" },
                { "lv", "772215dc4693940df18a89e3eb70c236b6045a57a34cd6926b9873133704f7a5569619b5a635af0140d2c25361d8073c60a0320f0eb885ea0985f497e924a158" },
                { "ms", "93e2abffb88342164fc73ca7333d89699b5d0571c202c762490fd09f327613bf2ba2e22e74263509b669d3ec3366fc100332f03e9ec444dc39f66bb642e37b3c" },
                { "nb-NO", "413f59377435fa8333003d4f6a7e5cb1bd852917f0763285c16bb428355e307f333a597ab379828cb279c832901ee00918882cd1285f32e78f1f9031190558a4" },
                { "nl", "919ac36852367f38305574bd268e1f32ba233690a687a8e8d9ede1b1d77aa72378244d969e3830971b7a23a0604939d07eb23a1f8ed97897c001c0a18f19ee01" },
                { "nn-NO", "1a1660d5ba0319145d4524f3a0a92f61fb2119087cc091098410ab0e0087d70bf43f3bd857c90cf9ea48216e2ae7708069914efc074bf6742ccc5d7729765fdc" },
                { "pa-IN", "4939768044e1f599b7af25cd4ea4488bf4acfd484dba3a6118b862f4968a31753f9df6615203b70ad23259ef3bcf2ecc90ff34a9975afb2f01360e9384e5bcb4" },
                { "pl", "4a187f046d07838bdc987f26d3d98e11ce48146b739411c0d85b6059ff2e59d2819d2d3d7b291b7bf84dae829cc37a4d31b491a6258b4a3ac9475a223bf6d713" },
                { "pt-BR", "e090c5de58dc05b5eda668e3d243d52c6451789e98f515b2bd9f9ab4accb78b7c5b4be4586ae98225598b8fe6ffc773f3062d263bb90817e4399f5fa246d60d1" },
                { "pt-PT", "6bb936bc7ef29618927ac9bb95bb4dc788a009788bb9a6da75b8a400d8a6bb510153ee77647d4e844bdf1b8cd5e86ae435b976486f45bac06b49741ec608e7d8" },
                { "rm", "9296fd2f2254deaaee1e79810e75ac745a0c5c0e480a1b50eae52bafc71e7c41dc4f1aeb7c1032e461a306155ef8b1721141f8968791cf228cefdeea005c1062" },
                { "ro", "ae3fae1e083f56d897714a2aa3138ddad94b4de41cc189e4b643b7a80eb842f0bc1a78ba8fdab463ebcfde22db020cf959e72728bab9646d6081515be7d366d9" },
                { "ru", "93298071b61044e0cf0198fa6329e9141e39bd8bcf56f4b437b87cffaa79657beae9e7d1a8358c410a09d344bf51e117ddd2d64d89e27858138a8660f87f9ca8" },
                { "sk", "f88dcd97cf79aece577e16c01385150ea3a31b39783a5e3177c10e5cee9a81073d7c20622e10720062c49645f10afdc14a6c14ea19502a277f1be23c5368a395" },
                { "sl", "223dd5753e9583a1a0ebaa9f6ead23abd51812d0679e4e5391c6c8354bb52e889da852b3877a7a487257ba1c10c3ace3448b4df46c924680470a5071f031b1e4" },
                { "sq", "4587d199e40775763ae4a566cd7f4486796bbf8b646b1fcafcef185cd48830c6061021d0bfb5390b54f4ed62ce16c24f43a92b37d97f7906145ebae99b138d87" },
                { "sr", "8f2c8db0ac390d41522d5b1b01d9f1473b06e458d64f8c56bbd94628d75855c4485ed2f58b54abc34dc4f606f6a319825d6299a985f4b08fe626dda59b336969" },
                { "sv-SE", "c6dac21c15e56437af8332ff92aa6d1f2a2ea90b9c95a969926b62e43d3228b3ab9b7a7028d18a04b3dea3aba39fef66effd60851bb16ec4d546a33aa62a3e8f" },
                { "th", "ed9eb18f4514e98fc66be989f4b6803e4561bfd5ff84eb42a38fa5079d7741bc0cae07f1d9e533fab98676442b0b0bcfb6065b67ccf94d1c7f68e430278cb578" },
                { "tr", "d63c8d01d7c640598ffe4008c867b5b2315e52e7113f6451ec089349335ddb709e90b8bcaeed2565c0a25ae836c892144738d6f65068f69c83d983e5e390216c" },
                { "uk", "1ef1cb51ec6758996845694e728800aa4747542f94f1c345ca2a70f54a9dfd8e59745b1dff34ca5c1a7e908deb41048799a511c2b140a2a528087ae6c68cd381" },
                { "uz", "872c3b2279d5206f6bd81193582af57430baf107bc2b9df960ae5fedad43286bb6f972518b2926832e3b750cd669452f8b2945deea8b95b788890b21709caaed" },
                { "vi", "49a9d5a614e68af0913936bbd9b820d7756d36a175c73179853ce2c91f37261076b4c3e6158b174b6ee08d4f576962ad645bc929bcb0e97a8c108db13f05706e" },
                { "zh-CN", "33ae262482b407ebbc577c1db4e15b3c50e0017e893394df3eca58db73b993d4e792bd7b7e36cd66ba03f44047048869f650eaebc681f20181293217b8ee923f" },
                { "zh-TW", "72cc195dd3e6237aab4b0e6db133907b7940fb6a84fd3313182c062b0ecefd23aafe9dcf526faa4aff16933067fc81714b459f9708be4eef3c453c6f807fefcc" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.11.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "aa2adce10a39ee502726acd9c78f48af8ef81e8a798a582b3b239753d1c6776cef1e7810f5eb5f1fc98ec3e1d60ca1fd1baa2da42a7cb04f672097484031e7c1" },
                { "ar", "32fe14a110f4197ea77fdc6b464d543bedcd6121bb12b45f8223a85ae63cfbda0bc8dcb5a12e0954e0a33e785dbcd8919f30dad637b8f75bc8fe1de857e26189" },
                { "ast", "9f6bd30cb779bd696581931586697979fa726d817fd892237d3a67f1ba34d2be313ba6d706d35dfbfc83ddbf0dd7d6119bb18680c81a327acf47e03b32bf8b8a" },
                { "be", "040b0075cfcb89624d491bc549bdf2bd1f9b54374fbfaed19560dfdae6def09bac3479c21712d29a5d724842d2f6662ebf0e6f7f00a3b6bb5f2fc5c5aa7e23a0" },
                { "bg", "1b3ef4111e2d0d57d8b932722fbe80f2af0dab00d67f11d41ea1fb678ccc8c6c5352b3831af1f0088c79cdad7f22f61d4c8c0f8f77cdbdadf8dff363eb18b6ef" },
                { "br", "a9a58bc0a34d39be72c0841eaf22c9da98ecee1d20fca890d8fac38d39bef3f763822ddb48d5936a5bddcd118ad8ee8ccd66e2f0a342e927f00995a04136cc6a" },
                { "ca", "89edc35d8e0c0d42617a91fe06ec0d0faee031520e914b4745ad391e3bfe45384ce379fa89ee98683466a63cf796f673cbe8df60bcbe3cbca2d358174a8bc3b2" },
                { "cak", "037393c2156b3469317032fedf9cdd0f51db7205c6d5196bacc380a61b97038c98e5967fba6a8c266d8f824c781499660417e469fc9785dd9653d936d3e19997" },
                { "cs", "afcac63cb4c127aa5bd09341f18b3adebeabdc17110f242262285b1d03affb49f29dde233c954c85eb6e2cd75a8a50843ab0502096e50e8528a6292eaadd1637" },
                { "cy", "b3daf955d50b7267c4609818c693421fcbc0dd1f866b977dad63d74244de2b8f15042261df8955c569b4dbdf36d6a2a62d2d5612919b37eb213769d0359971e3" },
                { "da", "f969e8b9a0957284f4df223fe1746e7fd3585ba2fe794395ad72e3e1cd54c24485159da85263414bbf9ca15e138ad0565f27a288700afacb72b232eb455232ee" },
                { "de", "a759ca6956dc5ba9b25bf7c6d04fb6b7e7ba490f94a06e8e3cf14734e3646c1f17040ebff429b68a3efcded471977ca9067243139e6074b280a6ee597dab9ef4" },
                { "dsb", "ae7750020157c9fc0d6a6bfd08a8af4a375e8dd12021b27835b3dbb3418895a01901d40be4b50ce7ecf6bd5f9b784538847c85b123c9b410aac8421cb01f19b1" },
                { "el", "c82ff3e9f557e61ab673be1c28aedaef53014f330db132cabf547077eafd44bd97e1a0c8c34b0f533941325ccfd6203a2591647104aa27c960904324e6335e9a" },
                { "en-CA", "effecf9e9974ed34e03bd537a323c0737c24bbbe1ebc9d7e8c4c3fd9d4ddb8ec5c04edd653bbe56a683cfcfcc31e41c62ff064b0af94330147c244580ad6b7c0" },
                { "en-GB", "c91e5f551ba8a29085487f633e8a2711fbc984e1ecc8895998604266db989bd889fff9e7b0483e64f86fda9c73308873e1697113d9e2a5a8a65a8c9a3cfd56b1" },
                { "en-US", "71de52b5bb605e28fd2f1f9c1cc79aa0b0dd503d959e09d77fa668749bd0fafca3a0d11bf64cc9f033fd957361d25aaf54a7d3f76edc6fdc4c8327f07ec95eb2" },
                { "es-AR", "025512555cd687f5fcfe32c08a8b7667c36b7e1a555b212dff8aca94714cce6f14d40cdb4d9a15fcba6befd03bf1e7606b82c4e8c1b650be13f4c9213aaeb4d9" },
                { "es-ES", "a3b79541bce46b7bf03e7277ea5a0594ee3d43d9549a3544906de92819f740b2167c399001b435cae9eb9a2ff2749bd75288b256dffeb0a9d3c60996297631cc" },
                { "es-MX", "4a2b97cef6483a2a32088409e8a49960e16a6b647df5a52de9970d0f1999f179c2bb1dc2f06454e7ff5934b52e73da0f73c87d4866c24a190fc34c2a029d49e4" },
                { "et", "17ff3691c2e6150a0deb49fcbc90c8a339e92238c84d814b8415a87b85eb6e24e2402de2fe617344c9f888a75580a81628a14f1a4d212365c74a2c183249feb0" },
                { "eu", "8f027abc8b94d751c4a411ed6f58e4491005d73628c7d51fed0bcdf9a94786191d6c83e82af9502bc8a8fac94b474eb5a426c07ed01d5d713404b9975576b13f" },
                { "fi", "b989863330fe98b0a20fae0bc4edfd907c0fd3ec9f1dd0eda6b52ae4269c7f66e6c7dac562c57c969999723fb81fcedec1035fdcb2901e48b8e9509e8098d30b" },
                { "fr", "45fc2ef65f0e3e977df37529972a7cf581a3c038087528de83dfae78a4efffdbab191cf507c1c4ae1c35c97e2f65bec28336d56b00a8e4437537bb8934955040" },
                { "fy-NL", "eeba43df0599b4538c94d8db3432f31edbf12a10a9e007cfedf2c24808b7e0853a24a9a23e3b41a8fdd82a32a4889c4acb8d4653d3de8bb36a5716460a15ff6e" },
                { "ga-IE", "a77f0ed241ed2b2829c80c8803bbbd7298d94156b0881f145a419e1b51a3aea4271c6b5aa2f64ae9e0d037224c154b9176a733c7f32aa6d88085f47eb534cd16" },
                { "gd", "cb35cc3993235d799c43eb2617804897bb413b6b6f5fa6ea3136f5838859ebfa092678f5920f30e31d9948555b262dcd4bb82152cec4f3c0cca9b9b3c184aba0" },
                { "gl", "7267db76a16c86e0e7eaadf9e3960d29ce10359b6cc52ecdf849238299383144b62d88f216bfe68c96863a7b9cc5b0869d9c7c137ab6e81d69e0d5779156065d" },
                { "he", "710cc154d7c8bd4c5235c3ed819c88ef2c536f89f676a63802daff9dd3b56ef2fb79320f41e7de2e0a0aeb87f9ecccc419f6cc0ec226d6e98d7c11d46ed7102e" },
                { "hr", "c87ebe680f4f663c120ec6f08067c54b15a41521ae6bfd72990c002a89fb77948bb10ed23fe3476bba620772dc4d7288c80f49106335f6717666af535fe18d06" },
                { "hsb", "06afe7ec24676e947aa47253fd33c3a660155c8dc2ef782403838ce2dfb2369294a34bff5c345a46714f547c37b42a41921d3bc7a30d411c3969c02b9a46c2e5" },
                { "hu", "18fbcb5c6b300b01f70909fd7103bfd76fa0982496bf7a2fa016fa2d5dc50a830a6dd0ddd1bd68632005e715d9ae74a505fdc6164ecdaa8342c131e3f908d30c" },
                { "hy-AM", "37e0548cc89bb80e3229ade112357d698b66a6e5b53b9b2bfe47d3f411abdff3c21cc8ab512165e68ae56bb161d99e117c08a8a9e86d93b3dc27c3e780fca4ce" },
                { "id", "c0be15a4b5eb6bcc60f9f83d0a2ebb439b32cbcb24dadbfe19b113bcb58e137f3dc5bbab308a10f08bdb4dd514da3921c908fea4e5adb190ad12dfdfe8da33e8" },
                { "is", "6562eb5f1a51de2a9453d7404a3532812ad35f12a0d0935d49f23481cb5a241ea9e36be87bd0668ac978534b448755f5c60af854dd2827a1f9c73e2eb1c0690b" },
                { "it", "1ba35f2ede7a7d94cfa22170f3ff116c99e17e1989198d160eda2952a6c6f512e0e7503a6b7ef1e17b31984ab432c2503766d38d54a5ea48e858c3b9c17e3fcd" },
                { "ja", "a9d089a05c634f7d26bd4483cb4aad75d084c174cc504e1e727ee55830174cfa20983cab705a9aacd4e0233ef2bd5ef33f9fa86e301996c9ab1b28416285485e" },
                { "ka", "57a97ecb8ffaa9f21a8d41ede2ef6f96cbe6c867778833e6336d7cf7d15497dc4e289e006738e18f9a0ece322edfc6a0a4e4546767209e86edfd30c9b26c287a" },
                { "kab", "a2f69d478246ab0884926c0cf6e8612aa046213744eb7ca3e2c27e351c404ad5f8d7dcfe0e362e0002f20682135f126de7504192401ee7d54c752434ec53ab1f" },
                { "kk", "357c508570fad1b65a140231c7965fe11f20fa401cb70ddf42815e3ab5111b2b8b4eef40bc578db1fb3eaa431314e58205f8534ab35cf4e4c63b9dc48a82e78d" },
                { "ko", "55b5c6ca0b792d9b97b473f264ba471f0fb0745ca5a42ff11fbee23c8ee2ecb68961e9968149a5cd7d598032a84033cf6df2759ba564764a73d8b4947bbf2b21" },
                { "lt", "265f53711f105a85718fbe553c82c62568c46dbcedcbade4c802020c7c3925de2c3a0df62c985eb9a6774c0638362c53c3c6a9b12ab9fba0a9ae22bbd80e10e6" },
                { "lv", "1b956e8c7637f51eec8e1d4eaef4abc5ae6da9e1bb132068e6f3c511ca21009aac26ff5641dd600ed6d9d5b73f03122c7f0d952f31f02bd22da9fea9411e2047" },
                { "ms", "bef4ff0a6046c560ab653969c4ab2ab1e597c7921036fe4e485e905b945aeff957182fb6c647fab99e4a4174a32f67e70847dce1f18cd69b94a3802a7f3fed5a" },
                { "nb-NO", "55e5a2cebb41c999c8dc372a385ce52c7e4cb138c99ca3949fd50a78a2ab9ab5dcfd4fdc9aa549ec8530f0c7aac28d59a2131ecbdd5433ff3e8bf0e9b4d749f3" },
                { "nl", "ae1342797f2095fd479086ceeab472a5a31a8905ac095bf5cee4ba1896ebfe56fdc2db47a1e9c0ce7f0f5861ca3f9c3f544a749049fe13e2507b1fd2bf3dc17d" },
                { "nn-NO", "ca033352330185b83ba4d62d2f55d6fc634df278d125175391bfbce370f2db55c40afb84df5944eefb097e356321b699209e20c60eae714048de6a3ba8ece2a3" },
                { "pa-IN", "f23ccc0fac1aef9142e078cdc34a560fc1ccdb73cc463dd05bf6e17f6ccbe84f9dc3a456cc38261ee10c9f83c350af85a3d2438508c175467dc2b0d45de6b7ce" },
                { "pl", "45a3559f6d34ba3fa0591e7e2aa87414aeda9887dbd9ca6eddbad939845e803f014c23c150b8ded94a6e2a7425e4bced07261ce68bd6880768c076422ca218cd" },
                { "pt-BR", "7de67eacc162afa3895a7fe89d525a73177f5173a3681e54e483c1b08d0c85275aecf865d1736b47993b8e6158d863ba2ddf54727c6cba0e112bc30522da376b" },
                { "pt-PT", "a8e55b67b0789727806c34ef0ebd72f8625f7e2f01912499b36c64a9927cc239c8defa259bd1e9dac7217a998c76e47320a8950d946e6d941d88175631e8bb99" },
                { "rm", "4a8e561e1ab5627c99d595009ec38c511ea7dda007d1023cf564644b981e89803f7afb4946c2aa9ce4f9e7bf6c86cdc9e3328f7189d40307b0aa0e085afa351e" },
                { "ro", "3892f3a059339e15b15fba21480d93949f94daba5322b5c1ecd83704ee85a5d450c47a6641feba9fce0174ba621d7ab8afca7b0825c3917623005f4baed5b323" },
                { "ru", "81b81179fd054c0f982e69892df53629eb7c00f330d976f216b69c25a299aa13183bbe0202f3d062b5999ca5e3c2764dfb5c94e89b40e32d43a5f5d59d1076e8" },
                { "sk", "f6b32f91c654a10e6cd7dc69a0013741e66eed18ba651198c8af612a5cd219c59c8076f58136cf9cfb3fda0e0cd2b76ab0fbe8758605fcab4bb7f6f461808ffd" },
                { "sl", "ec55b3c48474c2d72ddbf50f8e48fb85186e8d981eaa5b19e5efe216d90c9189e9e1b2cbfa3fc856b7a59ba33eafb63862671b14bed1c82fe97f6b03d108818a" },
                { "sq", "78e4254587f052d39f1c3d74c89c53af1ea669a4035a1486c3e07cc0f27db46ae48b0fd25765fafaa89f2ff902accebb9b73f3665130e3e9dba1bbfdad5fd8c5" },
                { "sr", "325c1d0585731be5bb00ff9935b02ce84063c526a2db84661f85dd90fc09a3e87bceeffc0942a4a516ae26457c882f554dd141bc89a15f9e030ae6b57f5c70c3" },
                { "sv-SE", "b4711cb3995f0e909d26dadfa99bb77a98146eeadfc96db946d3dee1e1be052cd27d1732699560578d687505ae84eb7b3fe4bac11e86faa0d361a872108edeb9" },
                { "th", "b4d8928fbef97eef6aaa9a30c2b6755499121a5340a3f9f909d992e14dff6fdbf6674ef3a4bb1eea3d1b925c17564b4d3c026aa6b2ec9f60ebaad1126a15c6dc" },
                { "tr", "5011efc4bcead32c48154e0782bf80b4e1a8a1b7c87dd9d6e426a3e5bfb61dafe4414b7cadef89028b3f4a52dc23ab398b96867b09bf25aa9213d1b3650c103f" },
                { "uk", "b9ffa714e7a56d52b5b22067b0aeaeb1339384f3e9e6d82660f7e38b7debee9830b504ab9985ed953887f9053f4c5c7dec6d19609ef1149e69eb68764ca901b8" },
                { "uz", "325d9dd3cd5e9d1aa5a2faaf3edd8e30efa6e9bdbdb60abaa99081e1eb165e9f48dd8705a5398ed88101bc3faa73ff5058356f4f5b2636980e4ec8a5ab0b03cf" },
                { "vi", "f7bfad4028331c2361802d4916a9bebdd42556979611b3d61fa895bd3702c07644faffbe958306f267f4f93945e1e21651cf95f70444f5de3e0249a9ec99fbca" },
                { "zh-CN", "b336398c4f31fa17a4b3faf883698f6d6f888c188a3ce58b9560be448d7d6d488cdae0a11114e767205c0c5a40b8cbaca3615653f71defeb1431138c4257bb06" },
                { "zh-TW", "2782543563d020d1382cbfd927bdf44e5ecceac1430e3f783de00c63ff6426d686b5c5a98ffbd8d999078f5dd47170658582b95439210c990a4889b52e0e1910" }
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
            const string version = "115.11.0";
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
