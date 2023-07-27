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
            // https://ftp.mozilla.org/pub/firefox/releases/115.0.3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "c1f1d4c28d49ebba38033a8ce8efc77647f40b2024a7a805bffb7ff7c4eb80de721ccfa9f30b496ca3a7eaca3321060342dea65021b146595f4d56bb5e764c3a" },
                { "af", "5f828e172a1179711dd674587e044a7dc8ae4b839effd8a30ce934a5af54e29b894cda6d91c1615e36c092e0d9097d6838edc48a9f283bc0b9c06d7e464830ef" },
                { "an", "a27b9419618cb773af4441e0692275b5b7b4a67e8080010bded1980a32ce99aad324ac2fc555ee9988916b8b22a1f8ab4bf8e35f00ce360d77a36166bd9f8323" },
                { "ar", "faef0ae65ae7a2928a03e4135320fe2a04e5808e923025be4382cfd9b4de6f0b836ea0f32ab6f7836fa4129465ff5b423624e6acebeac87a74e093a35e62be2f" },
                { "ast", "981496482c87f41028b0abe2a3229cc097cd00a17b8e45dbb382dcc44a144714cd732259aadaa9bae495dd08b8d31066fafd58fa7e2f7679ad4c7c0ba4b1d3cb" },
                { "az", "8ac2a4069b94c9efbe2cbb3f89870cf2e6464ea59f4760ca924f906e53fcb73d8e100f3533c7c5a6cf4ed8a092a4099eec354a84976439d917c8c9d5dbaf767d" },
                { "be", "463f2c3337e9d14fe77dacb2886751b880e039379267e8da70d15bff3c6d4e6839cbabfa16b2baf7da1d8b2bcd2952fe93b800f4629eade7b487aaee9e14dcf6" },
                { "bg", "d4fe0addc3e0fe5bccd6dbb4cc1b6bfa3e5312a24fbe0d7e055d40e6e0c7c52d5a98c9c2a5141051cc795f2c94b025fdb1c6af92b5e6337d84c2d665917e3f5e" },
                { "bn", "b1aaf179ff1662f3661a4d46ac2bd0e25307d50c52acdfb5ba91894f88f40aa8b50e068f89d375fe9295be09b95564c772ffa5ed2ef7c0b4edc937de0cf190e3" },
                { "br", "5dffc54b3df13c22537bfeecd6b6eb8dcdbfc065b2b436d894d146e7335b7e56f9df2b5cc9ef3c915a93152a0cc5fc69fa723005626ea7fecd50b6eb6bbaa342" },
                { "bs", "d9ba62ab1df4ac66bb90f7854fe55a3cea8f11cb837866f158335862f3f48122f1b74e20514ae6e51ab3db57d8e71ffdc5709eea540e3f979cc9473ae2026f80" },
                { "ca", "e5c95210d1a3b6d1378d9f474e7c5c0d5a719cd9a283eeb35e9211a91df2961d3144056b655d1ba7c5cbdf952e6a46c58d72b3873903d2a84b27d4c0f026e932" },
                { "cak", "ee249ac627931b059d643c81194c8a81602e83b03a05620993eda30babc011cd11cdd1d8a3e2ba88f423dc660a61e0046984d04ca0d6b98855b458246dde63f1" },
                { "cs", "18b5cda1ea1d1adf7197ef80b531ef966cc855d39a3ad40a6ff1cddc3379a7efe5e0bba361d539c1c97729b2aac72f139b795f766cab17ef19367af7f3dddacb" },
                { "cy", "f3ed5097a4baa917cb6cc439f693b6986ed73f176cac6a8fdb6b7eb48453b86af594b83bdb9414ff0d87644c2b5d8e9e2ddabb8f982c39b434c2c9c2faf183e3" },
                { "da", "843e00a8ac58e0137ddb9f1554d784bfd8d4082f614f47255b828d78dbb7e57c5d952717fe61ae5ade68b3cb5660744205135b7694ded49e9a7b8f3de9c25928" },
                { "de", "4c1178e0e07da5af5e777a71b8a7cf66ffd5a9d3882140b6eb8f9e051c4593a7baff03931ec74bd7e90018e568eb17599af9d07fb3dae2161df1693d5644be44" },
                { "dsb", "3faeb1840c6c20d60d0647379b77b13a162bc96858c354d64adc5d9a9962be790ddec49130d0e7332bb0e722fd92768aa7dd3b9274d1b6795a6215c96192d559" },
                { "el", "6d3a7c097bf7f3263447e8fab5969bf22da081f50887a851f2725449e1f8c0d6d14f634cb8da16d5d951a39a562cb5acbf7aa2dc298978a6e66566adfaceb85e" },
                { "en-CA", "daa6e972ae2eddd719f6470d0656a1d18cac9b80a1694934a42258ba1f5ad5192eaa5ca389ee4cbd4a7f522e181d87766325241a4643a13f434f7c8fbec704e2" },
                { "en-GB", "aa39e2c3997ae49146f6e878a56c70bb7dff82ad33b9077927e51889c997afa5a0a0fd7da9b71fdc3bee3b9b557879e056a628e78bb8e42709a453676f5797ca" },
                { "en-US", "0b490d862b0216881ce075ac21da374ad959dda8118e9fa3aba52f19dfa9639ae82ae9c078ddd8d5e0fc4bb883cdf7a6ebaffa67f18dc60c57c962afea9dfaf8" },
                { "eo", "03413f444d62b1c97c1c97cd8c90c4cca3857a962da0f47acc483e970a407c481fc8a08d896712134545caae45264e3960374b3d050da77a3d8d05ed2128f4ed" },
                { "es-AR", "5d077bb617265c32d3e76bc3d72a6ad210778bf60bea92fe87a7dc6b78a9341bf8c1be37c213e749aab7f427d5151b35842e7e0e7deda31b6cfed5d2de760719" },
                { "es-CL", "2ed11e491d9f7a64e3fe9fab4f8247333254026e9c24ccf8384fa3a517359b3332b2f19f8230a3f03388aa739aa6672b64112f3bb43dc5a6900db1686ef03427" },
                { "es-ES", "d1ee2d933a865184039b18489816d17ef044126d16a19994163968d77e78207193690db47287d470e74743170fd89954db68de28b5ca250fc8901e51679fd523" },
                { "es-MX", "01a3c40ffb1d4c36c6808fe4f84811b22e60d10a8f06153a549a00951e78079ac0b0ad1a565c9b6e4ac80308528d5670ea354adae55769289d45fffc1c44a6b0" },
                { "et", "fc3357e8f0e8196cbbd0293e2f1b2c36024c8281e865d1473dca8fb41069024494627d1349676aad85b7ad2431b3ebcf3f1ed489fba4c5586767d0166fd6c923" },
                { "eu", "9729049b24b282cf1aa1086a795504403feb0c1aa8344df665561650a06f318ae330655a433dced76823fc65cb68226ced4e09e16dfe0eff0f496ca197797569" },
                { "fa", "9946ee6cfa68111eab54179b044e66aa9f80d09f2065b2ff05e135e9d33eb583cc85aced8b4b62bd341c787d3b0d6aba3a01d6354a79e33eacd07fa610ddac5c" },
                { "ff", "e536e69599352d649d69e916ebb27f1ec95a8840bab6772e01966fc7a639db80059c52211e3f5c9d2f151e38886c98ec878021714469b36d9588f2702fc20b08" },
                { "fi", "7bdab63ecac1475683112d62adaeecc36824c3c3cdf09a14fd4ad62190c98808f891545b7a0ad250aadd4e359a6f46d28421c5c4f1e7ada337b92c12cb8308fd" },
                { "fr", "28aa65a96e65dd20ff542e9dd447cbaa00cc583b9f4a6a048a0c408b871927c21c16599ec58604a4c81fe59bb9fe09693fe058540ce60715993bb23be94817a7" },
                { "fur", "f2b8a7e769c43843484d6db7d4bf0bf45281742aa2ec7c5cd58762c0d1c5c58e9db71362b168ea93aea375bc6aff6b030ee84df71c737f3f4a2cb783ec89ea25" },
                { "fy-NL", "2150f994866aeffad451775f85bc1e59a8ce072222eaa9c04e73735e886536b5170ea3169a6199cd7f70f041e0865c15c701ca445fbac0ebbb3e86910424b64e" },
                { "ga-IE", "f8cf4b07bda0388e6194dbc7b3351860c749794578f85fd0c1355e43b8683cee4f249fc75efcd792e49e467f549581d6ab6e15d9b4ab5013ac6a2656aa9affa4" },
                { "gd", "cf6df68fea6583541308d00d72ad7bf6dbb314f96e0a73372f361f04b7d08e2ec305f741247fc3b3d41ce55332628261086ff0843890164aa05a39a1493e17bb" },
                { "gl", "912d4160795a916c1a1d2b927077aaf3b686a6a3e278747e9c6c9436c5ee0e8560c82c00764b08a161c506c9be82f1360b95e1ee9f115674a8ac2c5f1e7be1b5" },
                { "gn", "992dd29d666c1da67100a10bb612a535da87da1c9b59235b16041d7d76e1719ece62500c44232d82fc79715f921e05ea633071f9d1f78460d84f60703ff91036" },
                { "gu-IN", "89146663795cb95eb1aeeeac128d67564a0b59640c80a0faa40ba50ec737dd7f053ed3b1b9ae23a04082ffd8bd2be469c5d3bfa9b9ab27cbb56f3c1d99ac6945" },
                { "he", "2a2d84f7c4c1e2cfb54fc91936a8923d5ecface76726abb62d8125390c98203b052399ac951bbcf1e37b059d077ae05fb3861e53a80c10f999da3c09f50fd6ef" },
                { "hi-IN", "413ef6fa766526f9a02cd126dd300c7ef2e48f81352b07e1046fa28b6b15cacbe7361aa801a41a5fada258639f0db535180113dba7eac7d6f0e7f1ade753f4bd" },
                { "hr", "ae4816000d9c62dd2be41c97249125b13ddf332df163e1b72e4e21ef051209fe41a6404b5244160eafcf35adeb009b70e935753812b99ce1ff58f6c98e267376" },
                { "hsb", "57318f70a61de9cd58e1332ba2f049abf4f3ebeb8298092c839c19819cfe668b9dedc2fdafa1038789d329c01320bf385c68fbe75b39b4ebbf70d08262936b53" },
                { "hu", "706db1f069d4da417001f701a242812594939c13d831a5df7e2d36b6249554a2120bc8412f752d7dd46c62b612787b3058babeff93fb88d55e23d0a432fa0c89" },
                { "hy-AM", "c40ecc660f071ddc90471bb7667b29b6a3e6eae5f978e9cb61bc1cadca63c43f3a1a3ada4a174588aaaa7de8b8694d687200f6769389f21f8b784c10fc3fe69d" },
                { "ia", "530becca67bc98e01187fe7995d3d449ceb5dfe38dcd49c4162f5654194bd2ca9a5282eccebea47171fe6358ab2c7a8a4d4343ea17a597cffdd9418779fbb5f6" },
                { "id", "2df42dd0743b8ac5bcf4cae00cfc70738d82d3a9a3559ec0e98b5df8f17d570e34cd8cdf97858ebcc3b809c1ad818ab110262b243666098e71b631cb7fe1901e" },
                { "is", "f805248c3f94a24b3f2d3c471cfb61a80efcbee663d257e64e92b38a8930548a949e31742f918dcbb332eb48175eb3580d54b0207cbed112eca714f34bdbb8df" },
                { "it", "8e81313f3d43b209db0a8f49085340d3ea84664dce9f73aaf24c765d6cdbca36366f8547ecb8bdee0db776f6aa107c6c50ee5c9ab65b34c7da1fd0de09e8060b" },
                { "ja", "7afd36390a3524eff41e046d25c68a97bea2a94e17ab883419fe294f7fafff25df97770815dffb50a10f9d65b49bf16093f1a3afabc5e1fac3a0b28510b1fc88" },
                { "ka", "b15b58e687198879113753f58c52d09384b0c5ac7a13d302ea68585f1e54eb7af31a43aedc6b4de7b30352cfd0100d7fac858a73d9b1031d175f12376c441048" },
                { "kab", "ff585500e7e5ac669d8e08a1c7ed8dd7ede5714d4635b00fe692123ac01e13b5abc3b3c9fff7d33ee463f4051a3b85d8a5c1039c23b3e39de9a87e4d6b32a5ee" },
                { "kk", "981ff1dfa06200eeec14a88cb3e06f5ef39b58e919537a8f0b8c326439025e3950e59a24d2a210e071cd806bd7b304c7303fe22fadb817fc68c1988c77f6bbc3" },
                { "km", "cf9aa18f96fe9e223d60fc0dc56a6ddd9e04a9172fbc61c08f18398db64e80d3e0f36e335b5e056469117e093b1a61cd9d7c96a7e60b03a2c7c95df1f9adb307" },
                { "kn", "e89bf08339292dd627877e861aa375a8ccb75a3fa024748215e2b650c4bda8cf199b24a1205f25835e19717a92e03078a4ca155502e45167ac13d32625d28034" },
                { "ko", "d001a9434d0d56b415150ab188baf14b451a01ba2941be1ee701baa5f3afbcdef04e7abc725f0a02d3743b6926bb4848745f9ba61a5eb3fe8d181e67c31ed47c" },
                { "lij", "6b041ab7e45ac134c1be2e20942adaef081a169f82bbaa1ed6869fdb8891ccfe8ab1532730b4a15d6b8c603ae4706b00b9fd029e2d3761328f8e95563d772070" },
                { "lt", "29ec6a2fb2d29d7edcef829572b59a6fa78e4acacfcf2e5bc703382d863b1691e38b7e137787545d84efb7094974b322773f130ffe93312d1c6cee1edd4d60b8" },
                { "lv", "68ef374261636ecc34f568142d7b4cb558a16f59b513bca1cb17f78307d0dd25e7d119de78d75a4fae43d7d4ba449a1ea18ffde12a5371ec0228f42e9b6e0cbc" },
                { "mk", "bbbbaf88f10520efdcdabcd09add116cb68eede28553e5b5b066831e2d75caea90167bf689c385854d1bbcbd2cdfd107b1cb75295514ebcd872dd31f77e9f0b7" },
                { "mr", "90123494b76cc33d3ecf429ac092275c09d6112a7116832489ecdf485376d3f0456eba3d4b3388968218497295203feaeba18e62b99ac0751371f61153c9a6d3" },
                { "ms", "485097b5dfd1ed3c7d59420385a426e68bb2b2fcc082a4dbd2c1a21577cdaa9f9f3312da087f703335226cab66cbbb0e09059a9af963d995df259a0d3b61ccd9" },
                { "my", "8cc6437832269e4d58e09e959f2e2f6450704d84e73dc80d5d5d3577797685f21a3457f9bbbf97e843c4070a2ad341c4d08c928dfae88f627495911138a8e0dc" },
                { "nb-NO", "2d3356f2ad74146e3b147d23583b8e9c3ace45365e5b27bb8c71cf19645908cf6cca588e7b2b0839ec69af30fd2c568c510205f5617f37a8568943965351d880" },
                { "ne-NP", "e61abaee1a699176712d45528d2822d825420af2ce2f3b4c08c8dc8d93d753071ed38bb999e49d476e3cff6a3fb740a7478003232b578d85daaa77b5ec864845" },
                { "nl", "ccd41f0d41c3fbf26f0f59b40bd556a0ffdb4a37efe8d77b36cc4fe03d38cdbc208fb0f65abe581211b45933fedaabc0051e7e801f07ea603dd27d2629ccc59b" },
                { "nn-NO", "7c6c38af3789f12fd9272abef3d9a898a8235c907f3cde747b413291291a3b53fa86cd68ee9b959044a096e517b707d18e09b4f431b5450227cbbd414d24ad65" },
                { "oc", "fdd0d95531bc542a2596bdb8a7e6d1647683a54a6a6438fc8c07ae0e71b412e2083f18970bfa2557a9d15456bf53ddac6900bd5328014270305328b715868423" },
                { "pa-IN", "da681261dba52efc1934e2fa62ba19d4edbfbdf149ea16481995ce30cfc45aa7085c3b1ba1b858009dfa9e7474ba457fc4e151e8e01b825b2c7c41053c13d036" },
                { "pl", "d458fd5ad01ca3ec7b04d2e8d64940edd5d1927d53f471c1a78fa99c3ef76c0ff524a812e78fdf9b7f1c9196101064f22a9a907b01ce869c20572fde2b8a2449" },
                { "pt-BR", "0602c0e575d8f45d81e81d91068711b2333a5f57a4b4acd66e3ab8a1006836535ad298db13ec1d0c2f6a590c2b469f820d0ea41c5e62d31f47c116a3e9cf9798" },
                { "pt-PT", "c14a4b1004dd3f22657dcdd4111cb1fb84d5ed40873eecf22ee841a0fe9b15dd7fb1c88083c95bcc22c5372309566a6fdffddc10988c9c15780c245bdb816cd5" },
                { "rm", "5814670ff90e32b9a6a3897bd8fbaed58f2e2eae8bf1234e2055e5fc61449dfd88a6158ec09daf009f2896d7e85ca4a3e275bae6722904ab19718f855e4c5558" },
                { "ro", "f876b930d25ba0e9b26c6098a64d777d7ff6af722c68db214fa6b48464664eb699fee589c072048d0e6d5cc6ee2afbb984ab89d75bb55ae47b1516c01aa93675" },
                { "ru", "dbbb0c72cde0b52e64f442fec6e3ef1ce494ef1bc2b14b85f5f79143384723d0ce2f98f09204fc3476726a5caff23bc4b579b8500bcc5fd993ec7ced1235c6aa" },
                { "sc", "b3347ad62e954cee8ad75f03973186a6deb4db7127328a6cbcf63282c8e57fbe20d766b093c5d66943f012b7930f5fb1a2b863afabeb9f306bf0c9d23fd93935" },
                { "sco", "3eb0f64879908a5d57ed886a2eb1f43c367cd1090f83cce641ba0674bf0f7c28b76457df12d838b2ea8ea691eacdee7e4fd2f0a375b0c0abc5405cd90fa0364f" },
                { "si", "1201fb63d40717d66298bf4b08fbda91260c1d5f4039336ca22b7acd44ef8c8867dea746b78bbf8c6cadf49ecd915ded59c41fe875ef38974a5c78f538c9ea61" },
                { "sk", "177c0aed44138b31b895217240ace221d539ff885f092e97afc920fda61158daed7a4403e74632ce095c6ee6d6a871a11cd98eed29a7ea471e3244a381fbe3d8" },
                { "sl", "49b8fe264f83f93f29e241dee563b3434c6cecef285947943f64f16124d175848ef9db94b16822b7443f16509b841dbe5169431e57cc1cea8c74ee38b624f788" },
                { "son", "c130280584eb5c1fa89e60bdb69ec2ac447314f6d450495e1c236ef41dbb460bb2991123ce97dcfcb0fa3fc655fe50ad7a615401185135c8bb7faa8c3214bd04" },
                { "sq", "984a6ebca547d5a56234cb73ad5f6bbd8a296db64c32f9652d69dd6d8decb97b5d95f4067d9958554d586a825a1eb8f89444b9c2764e6e003e011b0eabc99ea1" },
                { "sr", "603b0b6dad60735c531e4407aeee65e707c3f72e30173e5e1a63769b0ce390202aabc716265afd6af641fce8e2ec506abb8d651260f71ddec7b629ee6a1d4176" },
                { "sv-SE", "c1c54c3c660e840aa8f8480b49823598d74528e7173b5d1dd91f71bc606c6efb8a9c98723e847fe197e21d47017f080cd71b1bcb52ce5d5e67a9faaf3ba744ff" },
                { "szl", "50b9e7ac220e5ec9f3236e66478ef8580751897666bd16048793f5a4f9a7a4c032c12bc2fb268b07bf7f1e0a9920fe19a700fd070dfad4772c0026eb345fa7ca" },
                { "ta", "03ebc46b3ee218ed5e959eb17e3d586f1ec04c38e856fa7029239c2fc333cd854df531d2a569e482aef29e1d8529c9dbb6122097731e6ea25d5115026a200cc4" },
                { "te", "a7815fda3be43dbb46ca16836f8ac49490845461d59079507e29067cdeb4b28e64affb862a6881237694af2dfa29efdb09efd6e77edbfae929d701794100c51a" },
                { "tg", "45714794a45735312373de319d568b42bf156f233486a25a03023f2b5900a9223af234f202978890e46fe66fe53507cbc5b65065d182b1389ce7d77d60128667" },
                { "th", "3881c7b41dbcd005f846bc15b420e075554c93cdfa68f2ae4a8e76f15a298819e6a90dc60f6fca5c4d9f21977c4ecc0c11fec2cf309488579c76451109595d9c" },
                { "tl", "1d687f37e41e18851dd7c80cd3b763c53d13ffdac401b7d062d3112388eb25470b7c447d4ce723ee6e16528dc72811b3dd3f5dc2aeab6ed739fa6a2a34186e1c" },
                { "tr", "1bdad8776b278635daeb2f3d8ad8e28b53031a04ae2e355d466a22249b06ab2ff76fdaf7b947d0ce38dcd99fbeaec3ad3a27351b38e913d87817988d7eae9468" },
                { "trs", "3e9fefda01ad5614bc7985425b50d8e589a7cde966bb3d314a508ad92d4011f6233c793a579c41226c98a426656ceda6bfa6695ed071c5c06bf63c1bf5efebff" },
                { "uk", "5f39556e8d3506a82afabfb856c93e12ced27be73a7392a818b00a1aae3398c2e476e9099e9162cc7a3c43737a77311d2c059dc19cc3afbe58c33ba3dc7e6b13" },
                { "ur", "737cd4027444fb3abc4e02b6da009102ecbf01bdc685a84027089727960bf7fe61cd9425ace3b28f387b28fb9da991cd3753a9d73f514cc750f279ad27c6cf63" },
                { "uz", "6699ef58ce553a208fb1447be9717dd6d9d893dc9a781ab54c48297f1b6a64f80261a5199b87b329a36d9612b1d92fa4ff17b2629ada8b81a98c8db2897ec6af" },
                { "vi", "f76b816ea275e336247764a41f03ec7e6b7f733151625de4030e788c0e2aba17415acd831ad56200cc5bce72a7ead82859f4afe6dd89ce753ba2f7edb7341177" },
                { "xh", "b355901d2d508db6cde9fdf9d1a17aa4fb1b35bb277157d97b91e667f8364c15df3ab28ad8bd698c7c18dae19ab5ae7a5b8a8cbd97bdf663fe2fd509b7a7a7dd" },
                { "zh-CN", "ec5cbd9a3c4c85fed7dcae13d349002d45c454a9d1923aafad4a185f3c9a04b874674035760998d1a923610c9df2bad2db013cea8e41fbecd77c605823fd9e3c" },
                { "zh-TW", "ae3ed8abe6eab9a1d469b0f2ebb78440052fd01bf937f0387699871536bff2162f92e003cfc29daa5aba87cbccc9af07f99724b1d527a72155b4b60f950e4749" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/115.0.3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "38d08751c27769a0d263f1889661b1d5ad28d54c8fbd06dcbebee757c46586b381afe452a9791a834ad91fc53279e1886972682fdb09348bb978bbc1eac59f74" },
                { "af", "29304fea789860c69cc9e249f904f0c6368d2a5242a19a73f7025a916efe57ce77dc10d8d03faee20a53d8ef3c52b2be7a4702b9ef6720a01e43f64fcf025deb" },
                { "an", "d1b1a3b03793d138ca927267c6012a7b37ee1f65aa97290146221fdf65ba5b3e4fcbc79dfdcd3eaa50e493ed4533b2343ba8e97d0d8b5b9c5b749e5e79435d6a" },
                { "ar", "c06d77fc7de7d7261d5efdb1870fbd88ad2a8723e13c7aa5b9d961f9f69172a4d53bca0b234f1c15cc5d58aa2bcddacb3a2540be46cb7d1ff64cea5fd4c453ec" },
                { "ast", "8c795274d3563d3d10f2a67a5f25469da723c296ea2414b5f215a21e8715c91f55875eda2a46dfc9e0c17441cad598645cdcba36fda7b30caf2a5509b60627a9" },
                { "az", "86e9efc4e9ab87df6f1cc57772090bd9d7935fad3974910b503c15495a40226d1e7340b4f069ce74023f2020159fe2c7b9dc6c2ba89460c94555db85c878cf66" },
                { "be", "b838bc11db1f92fcca88d8f8773ff8ce864367ddb633bd28b7165ce5f3ee4ddfccb755671475bab6bbb68131f922c3ffbc24b48f71c4dc4b4167f12e6ce063b6" },
                { "bg", "320cd84c7df37e11be43d80e5f281148ea10b033d2bf5f43ed37cca1fbeaa6293e71b95c05961c07247618acc0c285bc6ce74cdbf660262217fbcda2ff580ab6" },
                { "bn", "d1e6a1dfd3e0fdf6f5d986fe571f075ef80ff1bcdeb9a8cc5fe5126f5a67bfbf180382ff0152963175a7e2cc0144b1b2869ca4f6917d48e1c1d9a73fb24553f0" },
                { "br", "f20fc5804b6e765def0b873238693f4d0ed11feb44267eb837f45ee70adc3a9c5cc36c3ef37b165c98562b823870f399498b4e3c3895d8d5c29f3cc36e46af61" },
                { "bs", "93b58d2161f235d52a4d3768ff5240f1b3731ae44895811d4cb8fc88f5c627a19b09b4a53d4b3b6dee8446043def0a49b74bf781066961cf099a1cf33c83427e" },
                { "ca", "0be784ef3f9e4f9fcee0d8ea500501e71854e9a62c275ffdba93cdafbffda4ab6d023962d73136fc8d036d4ab1861892af4bf8730e9633ff5b3f6f4f695aea49" },
                { "cak", "cdf17f70e58e7b9503a210aa785ea708c44726d588920c33dd5bb964f918f250e3114e3ea98f7021b3cf768f7b5e10e9470552342e7cc45677d149c9a550b13b" },
                { "cs", "3e8cc70a9f4afd885cff24622ba6bffff514077ec3ebece82b0108ae23a690e17522d0f5e3e078bc6aa7aa5d0fc756e5adda419dfcb31c61d75cff32dfc9ef88" },
                { "cy", "25c210d9572dcc96593e8a69d0cc81b878441fc46cf253e678d8bd6391bb2375a588ec0b4ea80d244b39bcc53529cde8b1678bd56ca585e5afa0284771aff581" },
                { "da", "49058dc4d46ec30937062adf89115861abff14e808bb38fa1b76616d7bb9cb455ffc51491a93cd07014d162a78b39d24243f7d090dac49f9b8a3214a7234986e" },
                { "de", "4cd2cc909c84ab842634403b24880b349b32e030cab8f3b3e9457783bb4ac598efc151469412911b3539efa08c87e1e41475e467da973ebba60ac0e755b5801d" },
                { "dsb", "75c56efba3f8f1c41fb9c177425760897bb5285d3036e6c1c05a4618f57f0d6827f89f3d9fbfb942ccf90d0c16951f23d985c52ab8bd76da904ec0eaa09b6adf" },
                { "el", "37786d10df871005b23d545f9164b38f4be3d3385e48d7eb59b4345cc0a078a5c262f26dd01edb483294e3fee4cbd3d06625d86806813f8f0f3a6ca86582440a" },
                { "en-CA", "af49ad78e62b57ffa4a8207b94f8b0466e6988b7f227908ba7f1f2c246ac22f229f55bbdc39601c6aab2c999b1f5235c6800f24a6bb891de1f3ff1a8ff62b3c4" },
                { "en-GB", "b365ea4b0d6db2a26256d8a8684bb09086cb11443c26a114dbaddf00d9279abb5b8a6ad61481b57babc39c609fb910eb45ef79c2c5d2eca8b133960eff285782" },
                { "en-US", "51742cfda478a03926b62e639c7f1f0bdd127ee96f779d0c9eb9cb6b99eb06b741f9b69a1422061475131e9efd29d3ff61b7f7e1531725dffb33139393e5a725" },
                { "eo", "2343e9a665aa9a877ce53fe3e5e91bc37d470ce18f9f66157ff29c00ed2445ad0a4001e0fcfc2eee8cd89f24c4c92039d3618406e4810072a6f28f0767e78754" },
                { "es-AR", "503c6e2bd961b274f0f051f4cde3b7e19ac7338809d325f8373ec8c83dc5369f95b7f856c17c0725b47bb8e57d1df882c195deab80597bda659b27f4df2c59ec" },
                { "es-CL", "3e783899752d72ca959a7357f5e8bffe0cdf3caa115024cc9668c201222c205ea8faa5e22663ae8f0bdc856bbae11e149fc16022f2a65b8771d2d2b2c77b7e29" },
                { "es-ES", "329f8e0a4117c555589bdcc0333c7a54126afe6640e567ef976c15f2759693f73e9a54b502f7094d631c0966be9f16b64a82408dcdca8aa51d5190be53103209" },
                { "es-MX", "f6a9bf674b21f0664b5f8d60680275360e324646bd6b7e027d9d383b4cf1c6baf7a9821e091caf3aec428ebf3d1b55afeef3be3c880e0ce6735040a61a85d14a" },
                { "et", "4f7ab47a39d80c9f342fcd2c1db91303929ef42fd830f762eb619de8b39cb0111daa4cbb6bf0b5e039ef2375feff70aa7f351628b7a9a8bfe9447c523cebef3d" },
                { "eu", "8a5fe851e1f6b47af617f874985026293acd744293402edb1d874fb94e9577a2fb43bc8a923787d010422ef8977389ed90580f99b08733b5e066a12c36b9e880" },
                { "fa", "cb4f0594f20853b1b563144dc3c1e69560f5b06bcd6f9b4a227ca9faca988c94d14081791eea055be96383d023ba92588ff7aaecf5fea05ea1a4734f99b6e9b6" },
                { "ff", "0f472565557f7cadb49f4c7957bc664d33d5b680b4d7950750c0b6fc7091ed42575ed3504333975ff50b5a820a089048fe4dc1be156a57d83b208f446694fd01" },
                { "fi", "33c6e1b1b69f42e4e0088fdb57752595b8e53be3f485561e8bd746bf213350ef79a9e650947d561f7914ee301faa29739b4ec0b8f5cb9afd23210fac690faee6" },
                { "fr", "fe61b11803a0ff9c1c991f78a7169913b6c8a82bca935429c15d10b8cc4b29617875cc7990da9f2cf3ff9ae706a83b21ecf7701c71005577102da2c5474f3209" },
                { "fur", "7fca72ab2668963ba334bbe300d570c637d395a3a413c0a2c66cecb3354bf77d8d425f2cf63f110f4d101cabc624d69867788ab8d0eaeccf2049a65e967e59b9" },
                { "fy-NL", "82cdf70167bc202962ad8ca980d86b97f22a7d99d5649baecb8285fc339294306ce3228d869cbdc870001d64f750796f7b394035eeea16a1c17191b499655312" },
                { "ga-IE", "8b9df8d747d996aa32fb80177782d13494d7cb868ed29bd3aa1d4b95e3052a74bb40efee3cf1979c31a9151f81e70d131352c414fda68d987962bd7c2869583f" },
                { "gd", "f6ff839a977e60f44198db1b4875ad0de1af8e17f788dc8fa707f4cde11a5f405589ca6443877b08880a27befba7497f87257a48cf7fffc13981dad0fafa6de0" },
                { "gl", "f068ea7513474e830ea356fda2e8ea1b02c118fca385546eb811d5d0b37185c4ce582d43a8158ecdf512bafc32e477152d7a2d952e85702c645b62502c49ae47" },
                { "gn", "a3878dfa36d6f108b86d7e92956ffd6943970ce2e8955d168a3de6d45b8f221432416941a23c13c2d544b8fa72a4ff48d0521690d49b294c3ba27e8e3bcf9b1c" },
                { "gu-IN", "8088034ab465adb39251549d77f33ab050ae6a79821c2feb0dac47f55601b5bbb31e4cfb3a6dd6df4bb992a53f9ea9c542d3a2543e6737695a8a80158eaac539" },
                { "he", "71cf188bbe80bc3e3f24469531d6a9f979ce0f115edd30a749b793f3bc5480571ee20239c64dd287b64c5c3b94f06ab9de61337d62cc9c400794648b372fd9c6" },
                { "hi-IN", "5371cf240aa9946094051116ea50f89206e6d4a6668aeb841c95036922d777a2876846a7147b069972aa1d96144bd41b7e70f53527b06e416d0c7ba43c252e0d" },
                { "hr", "e38290066a38ebe0d48662430658d10956e2ec345a6b70a286627ef8dec104a730d890cd8f7b502890e2dce7a7343d4d04e711f3d210b1eea8d310b18513cf17" },
                { "hsb", "fdaf6da177b0ea92af665a2d4749e8eaa71db0412fd455b5dd7b8351e62c4bb22911fc4e220103eb7af6f8820b67e86f03292cca5da6993c321fb0600c7089aa" },
                { "hu", "2c91dbc0de1b9f5c688d117858ff1d6e5bd4c68904be627e50ee198fed9a75e421230c4d177659e5524da52e6555683f217d6e4224e3c6a638d115982537d7da" },
                { "hy-AM", "a59c12fae39cfb7557ee6ec5d03758123c1e7cefe72c2e8626cb06863f426e8589b9b6ec60411eb8c97d105c7d066043126224fe48a8e9ffde4e0ef3fbed7668" },
                { "ia", "49104baa0c9dfc72e3d21fe46f42a8246fb48e8ee8c9e950e73ad7d1facd9d79f08d6c86864282732fa4ab4c3f724f69a018945c3f104cf377eeba28ec8d050f" },
                { "id", "ee9a8039ead835f1a5ab7acc7a942f1112cfac9fe18c8aea30cdd3c2aa96c7ed2ecb489c6ec15130cb161644a1c6aeb1f8bd0b7214ddc7ac82deea53ad065b56" },
                { "is", "8357f964bac37ad680d07650231cbcd3e3b28a164325768cceee5662ee8e2d7f35bfaeed2590a48103c4baf57a4cb28e4d55ab550f80902728c25f2da4ede821" },
                { "it", "3b221fa68031686eb6a9a7fe51cb7f6c07cbdbac8d55c42fa9fbdc690efc31884da3bafd46272d0d4a5fbee28666ef777837a537ce7708b4eb984db80f757e33" },
                { "ja", "b3ec808eb257b6475dc4871bb05ec92a43133d48b6146ab74f06b79e045c9a422c9036c007ef17dd347c84a1e74e87e4f1c54fa313c562e078882746071857df" },
                { "ka", "f851121a9a083c6dadb18295dd9f2acb71f20367f995deba85dec9e15c0dd84b52a27bd31e80980abef9a52963f2efc4244d76792eb69ca900a57f841cd90d0c" },
                { "kab", "b529d15ef8f2396692ee8c8f9cfd602604c864e279c3babaa7bfd81f64dd38e460d1879fd519f2aeb36a306f8c0b32fbf5a16ec5961c618163892996b06816ae" },
                { "kk", "ee5b58d5eeb6064f1414644f7f7252c30cc540c4ac8ae093c91a16616633eb9447baa7ddfb485a0514b4e2f5fd1baebc8e238ff2f268097081105d0b93cbf50f" },
                { "km", "5899f1ee8b1b671027633ba7c55ab3b21526d284624f29b40d1eb5691865e4f96a7d635aada92fe0006ef0b2e8804a840a615316add3b798b5412a87abe48b8c" },
                { "kn", "b08c6ff0585db8be041d13d839dac23a4d22c001728ed9a103e3f557c6de8f79d0f2f19531f4e9df04efcb1a4087c13e256cec56a28b293cbedac01c8e0d44c3" },
                { "ko", "ab6e209ee953a3cc8c062c29fa6d52251f8de7c609ad9d2019242e477f1aac26bc97cd7b388b1f27e647cb6b78019357ff77eeb6aba7ddac1240026d8f387518" },
                { "lij", "a9110b97b0fbaedb50d33e71dd2ec5d5e079e8b1f7c30406e19cc454b59aedd7762b954b8a17962e5c1b18e48788d4f64501153fd9406d3520e807cc0d769f5c" },
                { "lt", "37f4e490bbcfdb4d0e421209d183ce69b6c972ce8b878cf813fecf3529c0e1dbd5ceb0b9ce93fac523b84ab3a3785d1daa348d22b6bcd012e95353203aa28826" },
                { "lv", "9d73af181ac5f99d426473a6fc1d1f20ce37fb42c4c1a797a32fb8f1ecb5772c7986de99fe07cf6f8dadc6bf0ca939219fef7d1eb6a0856f001209d6b16f8fa7" },
                { "mk", "83d275849dc4b7216604eca24f1eb2b37298e1c82b2eb0f404521077dabc2e26f23ffcc147956097c22bcc031259731967e11ea2ee17150692e8efbf0c5e6f93" },
                { "mr", "0c53e86416ead60ecf289e9fd5ecf848ed61dc9d1777cbef9002808f7953cac34e927938c0654a03976c6e69c7c702eaf7757e9c34523304b0964c9d165cf0b3" },
                { "ms", "137906361b9f131d4b178097b1289ad265009e7d33cc43a270b2cb922b41264d3108bdc5e1cac2068f02f5dc1c2026aca2939b665647bd4b513cc599a149ba6b" },
                { "my", "008d10f4a0432ed8fdd84fd008a86c3dd783c1b255cb69f0cf6b28c8db6a38c939a59eb65a91d73d2057f6afe65099046f2c01e63dac3ff89aa12902a61c9e0c" },
                { "nb-NO", "57422b49510ef1dc958db4f9e1c1817f5ee06484c67d8c737e0d580ba5aa476be5985a1ff993dbd9d5b34490f760fd36ab89fe73a67fe859511c5fb6522abc4a" },
                { "ne-NP", "7627a6e51fa440742d8e3bcd3211cae99af77040ecc22cc1b5f0d15701da046250586a656ad7f35b15b84701e4b726999d454d1147cb499f8f1d9f7703fe3a17" },
                { "nl", "9e72e5dfa4f9e325a2a15434a64e5bc2e6b588ff6a35b28802a2d5388cc6853ed61bb9e5df37f6456b0baee0d7c446ca57e918b37a75a5f7af96ff569e308e07" },
                { "nn-NO", "5077feb4fa90748b3eb30b259148c2cebabb242fbe044b60408b66fdb1745d4b45f19d7e6193eaf42741d8d58d142c2f64dbec9e0f0a68d3a94e16e272ccb2a2" },
                { "oc", "a520f8e626be74b01f2d3eedb824df8cf87b0e072d44adb5279d6c1e29e8b00c6e28004f4d18501ef4e0c4ef842b2ad7d59e9bbf5b0a485620ff80e058cbb605" },
                { "pa-IN", "6e2929add7fcab688de7d65c4444b9c4350da2efe48f7acae8864f871623048600274788e2d979142a3ad5299395dc36b572ea4e9d71e535fea91ff368391469" },
                { "pl", "b8ca81a76e55cce3096eade681f3c590ecda47ad3384d5a78db9b7ef137a9f036e4eb8d18c29262e486acd39e4c562078a0c38799f7bb7f20b8bc757f9e294e2" },
                { "pt-BR", "636a91a2fb4cb56257280b31e34f7f02523b8fe2a8350a2e8d9dede93087f91c4d9087af7e0fc5f91155da2a16e7597402d87b34702d56f29851439509460afe" },
                { "pt-PT", "8a16f54f97d8bdb15197b9bff11d866891b1cfd4ce968e06892c206c2c6ccac5ecd1cb98eb102ce25f280515c7398c690452b59b896dff4019c9018698781bd1" },
                { "rm", "40304eef287257f06de0ba0cfa207a9e4b60d666b951842fef2f3194ce2bcd5f7b2d2334e1e882c016c6f2588efcc077c0d6e281be1bb13aa22d43298b08b72a" },
                { "ro", "e246573c9223299a660639eb2ee0789ac22599c9847494055b0750abb0efd1d0b95c88310ebf20d72be060661e516b8bd90d86ebe841c0b923b9acc56267ebde" },
                { "ru", "b2cfea3c06a7dd6c56b7df12057f2f43ad90c4ffcf24806fe6bfe4efc61b418eb3f5d0ee5ad9a830af02f8f8e8876fc640dbc42150498950deeb9cc66ffc7c52" },
                { "sc", "98946a7e228b000289732042e38e7996dbeccaea3e2836dda20c7ca2e9b390115c889497cdb99f0d0d8cb4f83d3680192270850d3da2c823f447ff56d4374878" },
                { "sco", "99fc43caab78b8e4b66c224be5595f62ae5d9c18316df3d4c810440b99054dd32071dbebcddfab1ea2164884d1481498eff68f0bddb3f98e555858a74ee4b62c" },
                { "si", "ac8170cd94a4e3e06a2dd8b19b36a4010af1a02df264c8b0b6c64e27009dde67ac4aa059075e2f77b72029dc6c8d125cf635307e5bc3ce729ea0e134efb61bab" },
                { "sk", "77d9567be7bcbcf3fd8e30c4cc263fd616e4db971a75af66a7c4bef33b0a1d3c4f2bf57d94ed5d309afdfb9e4860697456bcc0741660dffeba86017be0f2c07c" },
                { "sl", "59a85efcf2672805090aa9b4012ffd3d7c74e3da1f60389aa4dce1bf7c73204d7f32dee65048c0d0e9550c6f3283726dd0e41988076bfc231b00d48fff8e71e4" },
                { "son", "9029b99f82124c9950ee4e256a5291c178dd70401eafa338697c2d63ebe60f6412e87582e0f4619c71e3afe733cc240502f975e4bfdd230bbcfe765d89715931" },
                { "sq", "da10965c989d89cdcb30eb2c8e1c4c29b50ae2aeb64814a286ff940ef05542bf7297a89f3435b533a33ef7a77814798220c1fbc95a242112af33a4b4ccfec4a8" },
                { "sr", "6e8d40f56399d9abb484d6bcc78486945e22797a7788b75f1d798941d74cd609e295584bd3ba9b3a8e7834ec6eebcb01e44161041b3ad24b67f13ac1695eac3c" },
                { "sv-SE", "4b385d0fea754eb50e7ff8f48407f89f91ca3be88a0da320a1e6de2b72859228b9d340afd2e5651ae08d88aed7309865e47b663b4cd5902143e3740f780ef3fa" },
                { "szl", "fef5860b42d89dea9543f6ee7c1662e24719bd7300236cf37d829498832e5a42a84be10924c4e6a18725498d8bc13688733525b40848ac700a36730106bc6d16" },
                { "ta", "3d20aad1ce2576f0dcc910a150c4a689658ef9b073a22e4dbdce535497dd2e0b7fc5bab1def94e5fdeacb21cb47d41d9bc95b05e6f1fb7cca7d3066e08e31edb" },
                { "te", "7b9b045df82254c8907ebce7e967a65ba1f97993b9caad6cca050b24b86e560bb04d771fadbcc4708ac1dca8a539841a3177c704b2c344e475cd92b27499ee32" },
                { "tg", "43f32495cb52cc73074a28e241218ee999e21a11e103a4676434d2b5b19b9ccc20a744af31fe2d4fcf1df4a476495cd42e1ddf0ee8420d401451140e79cadd8d" },
                { "th", "ee198ffd3f2898abc8e2591012542aa249926fc218742b05dfe6ab5b37385fb15e07ccc7e9a2c02821fd531568a5e4d2164f298d7c6065236ccd8542b036a793" },
                { "tl", "37b55055b02f4ed0c49d183828ba2ff77fc9a669b0d77be0170a8e46ee092f63bd2ecbf9228f105c69cb4c5239b489539aaf75688118baa43870ca5c3cf8642a" },
                { "tr", "4edeed3e19af0b1a31597ba736b9a78db4cc0a0dea82992b2c8ad0024de767e2a92aa058ae681478b9c121c98e304a41e20038f454c6e65f74834c6f71c59c62" },
                { "trs", "d9cce2bdbffc1f6198e02a272b2d25b3a205b18701167cf13bc74ba7da333e5eb3c426f9e62a30b72bf960b0a34473e8e67ca6a4ef4e9c3d340c26ce8eaa399b" },
                { "uk", "f8c02d4ccf9bfbd510cb3758543de30d085ed3d4c8cf3a273a449d7830dc73c8a656086f242a7b12246b0f47cf7f974db7e40a3b315d48abc459d50d74974e2b" },
                { "ur", "c7c80f221c600ae6bacf9a549288f4d5e309881d0ece8da6990dbf6eecfbf2b0ae0738d7d151d28c2d63cf38fd5138885d5b20657c9fe188e02fdb58bf359297" },
                { "uz", "0b46dd5fc398b5a22649343bfe045be542db9c426abb4b8e49d0c73ddd46487e708effa279aaa3e257e9587e30fc264aa3bca56fd56f4d70285da9300403814b" },
                { "vi", "e7763540ee91b79411e2c5409247c485063a17265e8ba485af20f96f7758b0ecf61c8f0f6d402ae1df81b067fa228db876c9873f92f10bef8199355cb952194d" },
                { "xh", "5abb0d1a6b00c1094d4d588fc21c90bcd9acf90832e16ecfa7875fcf4f11c37d5c1d6fbbf5fa7046bcf5f19ae4fdc43d9ccfbd9f8d6474802936b71e6d05a8d3" },
                { "zh-CN", "e091315569bfb2bbfc9f7820a04309d47d157b64213dab238ed3a17556c40e744ede1af65cc485e1bd70d896bb7c9822326edde9cfa3a52fcd22a795d54dcb67" },
                { "zh-TW", "7b2d9c654bdd346b265585e93cd749a79b2f9ba91199284dbfb4ed268395874d22a0ad1867ba8ab21a845a463843a602b0f686bb2c51e87cd352d774d2db7525" }
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
            const string knownVersion = "115.0.3";
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
