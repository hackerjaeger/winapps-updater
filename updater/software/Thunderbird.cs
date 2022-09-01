﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "72b11edf8c1450793bfc649b37aeb75823761004a8288403601710f43a2cb3abb3f3b708203dd77c6178cb3d04ce63680c3924b16157e4a966db7d7ea483bb05" },
                { "ar", "50a6780ed41d27e42a89532681ec8ab138beab24f58a64328ebd78e616e8a915557ead2555ac24093c8f5be335143899a1b869c42c908b0d91dc45ec9b8eddad" },
                { "ast", "7405d0f9cd6ff29d2205114cb58845a7dbe2f012ab7564e0009b5448b9b5a604942b6253f02e5e7b1662a7c4fa56ed193652ee94f549df20bb767ab9ddb029e1" },
                { "be", "1a3614cba6ac0b09fb4bac0185c54d7293fa81e81098a6891cd475bdaa88abe0ef5fcf5cf1127c364da378a99e9ec6592a0d65340a22ac905f7efffab459fdf5" },
                { "bg", "b62bc041d0284f65ca52792ef1064388e7a3f4a7ffd64d48ea12c7a4d3de2f1c7a87137cc22c0ef243440d5071d2a37e1da41cb1614e1d376389b042e98afb80" },
                { "br", "01cef66412d7a1d58023f1d1184ef5ee059ba14b1f5e0d0b59622419496a764bd6e2c1a59dfaf3ed52d106f26b687b4e39ba68700e487b6594cc8cd6f21e6a88" },
                { "ca", "c41f36a9492ade6065b7c28949bd1c49db754a20247a61a5932243db3b5cd587585ec8d4d4bdee87d49153640f43f1a6eb02a0c57b0d0e9bad64f2e50fb7a514" },
                { "cak", "5be08272827bf26c1012eae4082e759799bd9f95acad9575e546123166e36ea0b0befad7e79aa233edcc49c227f28a7fee70a1834579367676ed71c70d538d3b" },
                { "cs", "8935244c04285a99da2a6b04c69af82408dfdca6c7cc2180af3d865318b56c9eb2d63c94bbf5301af5fe4058561a9977771e4a4f4a9f74345143ffd13f033fe6" },
                { "cy", "53113bdb65af9b4215ea6d2fd057a4d61fa106e5cbf6e21f627772455ebfb4d942c881cdbac16797393787f53d25679aad05d05f0a7d1ca2516a39d008825e70" },
                { "da", "6ae89105c35f1792f7e0e66565caf69866d80d49054fc0e2e2115542835dc08635e61dd77f9ccca97f17e3a9afdc15b7f43e1d31c24c568ef536a4b7be8a0e0c" },
                { "de", "17ee54ff847131e5e9e15a53dbc273fb79d45e9d49f3b2caad16e36cc39c7566beab74e4ae273d508e45de756933e2d838bb83a67712c59213dfcf51610406d1" },
                { "dsb", "feb6fa816e36690a1682e3dea5c1a1368fbeba4e85917c2b7bbde149b8e497773dfebce83458d6d4be8b66bd0288644a69e2529cb0be1f082c647c085cabe979" },
                { "el", "73cc56209de6904a6f04476c91680294d79bfbcc3c2edb1f5362e26394b32fd7f6a17d5e64d15f2a14257c88cac58af4c17244e961902a0377de94a3d6473cf3" },
                { "en-CA", "93d237b0540b6ca1c42cbe7514af2072f15585f6b3a16645b23d658a5d02da937d0d6281eef8ff14c0e815dfa0d3cf8b9be27f555cd2fc3146322310dfcc1050" },
                { "en-GB", "797643874da631c95f6f6d2ca2ca2d634a448726580f140fb8a5c4ac1e97ffcf0a2b794b410b1fd2f35b5e1dea99b37288758109c6bd0379b4be62a9c94f699d" },
                { "en-US", "669579c4ff8e955bb878892a4bdf1a968b4ea25dcee1606816dc2bc26d4e9b104fdbb00378837479696901331669b3376737e7856c5839f7459a6a43bb7f0dba" },
                { "es-AR", "683f7ab0daa402c06d552f9724b754ff4e33ab907f21c3bf8e89872b06dc0c51faa7beaf648214410098aa8b8471e7a380328ea2c3f61fa4221cbeb24d344f8e" },
                { "es-ES", "b674f925a1cc8fd8a6018cd1198c2ad2dab17835952dcce00ea84420cb812b07a21c74d6a0af2df93d8ffd1f05f4d1fb62008c14c1d1aae4a665739f75820b62" },
                { "es-MX", "8c8d1558d37f4950087d775cf8908926ba3f03871723e9f92c2372d12c37b57359ab73e3143c49fc2d9845711f3fe4c7a0fb7a3a2da993c64bd560ca0f3b2b79" },
                { "et", "476f28c267b02ffa932e1136e21b164cd1e742dbc9eed668f8f4ecc17ab69ebab60b019da03e20491f8f83b5c3d5ee6b8296bf41fb82c8a906cc148ec5dc525a" },
                { "eu", "a453a66890b2362f1a745e16b4212642af634d0b8bd2636842ca999ce523b7371d0e77fae6f64976f315dc45f2bc13890d5a7a8fd4d03fb2f229f60501fc2798" },
                { "fi", "29504835c5d667f79d99b93ad841184c0fa96087e946a6bb4b3040f463354b19317b05852c397616980d08909e6a5afce31a084f00007503f183ffd7a2c800d4" },
                { "fr", "b3e322c877c966ccf6b1a8dec75f1ee4b2127a61969b3e72013a97a7907a1223344df67aead37004f49e11b39dbd23d2a505c8f7b265a4168d27f81bf0981c58" },
                { "fy-NL", "4d4d518ef84600e826d7ec74a025e98fab60b25ee6592e8ed448113a8c717b62edc49ce8ae5ec3030ac96afed62d782a89412a03327e0ad650327ee9fd9524e7" },
                { "ga-IE", "6f812529286f44a3e24acdb688a09d518a9a41cba0e3ece58ee9a05389867606bfcde6e7bacca8f9c3d177439a62fcc8b34d47efa5e6c2eb31275c7ce9b9c181" },
                { "gd", "70960f76777874ca1ba72885c01eed81e3fd227d159ac8d521da140d4a65e1f4c8c88422c64a8ca35faad99217d4dcd3741668e5e3ffedfa5fc60477138a1bad" },
                { "gl", "fcfa12a7b771ca7a7cf42f07ee6f73a45a8a9523911cf329303e316c2d7cb715b6204ad749c0fc3171805c02bddbd50213775bd77a076f2678fde5e41396cc86" },
                { "he", "51d18d746cf0ed0350b172fbdfe4489ffb84a12fe2fd90175c9dce1e02b5982f8acae5d074cf5300d0f512436557e1030a4e6243aff6528661ad969cb41d67fb" },
                { "hr", "69c0c498b625e26581c56be6d2abee2782460b2fb4da0f088e51d421a5cd1a44ff7b4b5413b541539e23303fed7faebd989782e1e09b25fb1f47608cbb080a54" },
                { "hsb", "c6a0f521c4fedff7a3e64d7fcda981c0aece7737c9461f628eeb0366459326abeb9afe6c39ab2d6e5f79a282c10ffeb57ab99a42595d6f2fbdfe2a81cbd8c4f5" },
                { "hu", "554c1f6a0813b5e8b6e76d503cc7212f8cbfabb77301d8a67d4e4aea14224e0058d99ba3629e4c003d3eb27005443ed2cf3d96bde856e5f61a42f62802a399fc" },
                { "hy-AM", "5101ee8a0cb05bd832a2b861cd0a82033a80a340c6adff4d887eefab040c11f39cf0d54c028a68a0e171c4d9909207e4331f5406e7d888b5f62768238c6ece64" },
                { "id", "860d8232a5bf8ed074403f274662c1c2816fd978d414763b7d33dd94b45e3a2e94ae25bd3b27b244ebb78b8e75d952a4bbec8af55ede45e98e8a3de29545b661" },
                { "is", "c767055cee800e6306df3466e7c75a6a0b49104fa7373edf4b77fdc2d1fad8bbafecf5a8217d5e1ad4ce34a8c0b979a56684c2a9c9340c06803aad2baea3bc60" },
                { "it", "d8416377025bb2bb7d4aa9cc3d1fc4e07cd79930cc8da4f06fb760a1c185290734b6133c8f9c34e15da5c3f4a30bb5b7033a0dd4732fcd6b26714c23ceb197b8" },
                { "ja", "eaec8978891ac61210e872990a158856ad3689588feb9328c253605674c764a12668b4c5685719109adf1ff0b057429cfa37011cbd53e4301f8adf4c28f8268a" },
                { "ka", "a27fc7cbcfdd115bbc4e85fc350b21824582d06708e0724cb79d9791abbc60c41c9d630394747e78c81d15eb930ff520876227da0daf27e4b16c19ad542cd031" },
                { "kab", "9d2d5b17b36a5135e6395bc65ab45c8ba4b329b557eb5196c7dd00595256cfb16f290af529e54759b7257efb981ff7370cfe5514e9be273cdb10df175ff15712" },
                { "kk", "31c98c8e39410fb699e840baed5d8c770a41d4a4c7679ca316df63c659b4288de8cdd9a638fb255d013daad20804923a97f5133e3fba67e8015dd82600add638" },
                { "ko", "ed3d152ab1008823ee7a97f7712390d85a2e86a8cba665c551d79db9ec5acace70e9e4d8553cc64c7e7aab28cf344defa3cad69e4c1e901865f3c38251c7346c" },
                { "lt", "9681cfdf674e59edc13bd22d605e6427b7415f09273e23027cfbce1c369e7c835c7cb3273cbe5a069865879b51cb6c4c170928f5ecf5b456d66f6df448a94174" },
                { "lv", "43378d0b11fa4c4114270cfd42d315a0e3c41a6b3f90e43d901609fa06d3239174cc8c8918beebd95b8e2e179acecd2ca3481aefb4d8870e2e546c6ff63a3871" },
                { "ms", "436ffad6dba8f24202ff8c6062c5762cf0d5bb7450eed46afec827d145add17475bbc17be2b17a5f4cc00dcc2b2514469ee482a34601f940b31edc6d8744ab9b" },
                { "nb-NO", "e8f7a4018514c37e541bfc425c0db4a81c2d972e096ad29cb41a7247cae64df97050f36c3d2cc3eba29b392c6a476a7a17af1c6f8532656be32cb00b35ef92bb" },
                { "nl", "2e4272ce021a82532a309c86a2fae449f80f3cfe7cc44eb6e69b60a108f46b00661bdf711a01c83941b1596200e45fc8dfbeef744ddd7e8e1f58a5c6e7f8ecb9" },
                { "nn-NO", "e06d51548585a5e26a80e0b7c119a6c56ab8a515f20a06479e14b0a84488880e826379ceb2d7fdef0b387dca7b17a6a14e21c3ce7ffc7168dd7bb87ec8ae0cd4" },
                { "pa-IN", "698b9d7b0ff487c786901fae9cd6fefc110a8c57d7b435d26952201dc74c2df8d64f507b7dca095195a8bd688be28b69be8e67b2f7d19917b475cb5cebfd00ec" },
                { "pl", "099a157315ff344cc1d51aac02120f60e26717f7df824962525953970a87e6b850ca9d3ecd2fe72bd70c58b46ad42ad07f15fb74519522f754d20e44d1cf546c" },
                { "pt-BR", "ae46b282e89891a970292b60bed0b0b41ab542890f1f91c158c505bfadb7d6ad5532aa25d75d6c92622eae63d336687b5abca8de1084567aa4a43fb446908bee" },
                { "pt-PT", "7f277fe4a1b4e4d811ac3218f1ff190193d14f50fe01d0b3f8886f13e1d46a5f57968a73d3aecb58d05682656b8bdc032aefba8425b1ffc70540f22dbf4bf4da" },
                { "rm", "58948a1b32fd8b4137cf12060ca8265ef7c77e2f6bfdb18dfcf17cc2ba7eae9b9be74f8ddbf5b7fd5502aaabdc9cd1f7790ad84beb0fcaca0d472a72b3adf5a9" },
                { "ro", "b20178e06b345bcdf05e0da0d03c50910f0d656892d97cfc62cdb9f950757cc10d6e5d2009dd0f9a4fc46835a0dd1890151ffa00e5e7448715db302951fdaf0b" },
                { "ru", "c3d3e3226de8567604b4be8ddc990a470befe9f5f2dba79b917fcb6e3f8b3f88c45da2b4f2b5e79af07cd598032b43f6ba169f3df642e501fb0004b66818c80e" },
                { "sk", "f5697d9139fef0023edfd14f1c66cee4b950472ecafb2cd17f3cf741cca5486ece28f008898edd57194782f2ae03a0743be354e44d28c1b97ab352f8c8bd3117" },
                { "sl", "4e6507b534b93ed255eed5f0030370153a5379844b7b65506b0b5bc2ed585a3137656c0e003cf23e3b8bc2cef40df5dd799a0ab76e1181b01d503d2471a15642" },
                { "sq", "4be5154f091b0e2a0c1f4b18114de927408426e0025665d0d726d6f7b1d49747f173215b1d9c1ccc5cf705c88275aff76b2c1479cbe14bc2bd4f6d63f4808f83" },
                { "sr", "c92c7bb9152df05cf0069e8fcf9d8dcc1f07694e95d0b984bb5b8c27030a9dba2fc360307fa4ca49d6b30b87cc000b9f9f7ec58f15c1b4a13693d3292a8dd6f0" },
                { "sv-SE", "ac8a864a3a4c7550c945bc74f62f1debda11cda5ab8150632124695d292f5a9bd162485dacad5c51bc4169ba7fe844c052f4b3bd8fb6f439dbf338c816a95fa3" },
                { "th", "e900394492f79d96cba6313b96c39127f9a0941bf362f3e7ed6800933f7385a67d668bd2d1ed7376e93dcd676173fe2a5d43726f1ab8adaf4eb80fd724c67cf7" },
                { "tr", "a9de84f0615881fe6c48e16f8a50f6b65996aa99b4d6500179bbf8e35d94d1564ea25d5e3c6c3567d5224807f388bf195f3a3f67c8f3b0352e47ff88caf82162" },
                { "uk", "fa66ed38e1925b301e916dd34f573ea1cb0f96e354d138bbf1f529e20897b9198c85b4f986172b15f872921579abf15e06d2c774efcf585746db996ccb0c59b1" },
                { "uz", "1ceda768375387f0e7cd79cfd96bbe522dbcdb02f5691127c14e8a4c7a3b247a41d92aece81e8e098c9d1c8ee1d221bb30361bd4323689dea397b0a0f9ef12f7" },
                { "vi", "c2e9069803bc5bbec518ce334a60c604e7c26b014c1e3e62093ce325f5a3e7fd1c8237e761830bfacf19695634b281e5cf9c278d66c9ea92fb46bc66159e7f52" },
                { "zh-CN", "c9026f324ee3db3f2c3b12060bf5bcdf2fa5ee4f7a440c5b649997059ab57655e64cc5a0f9ee395b4819c207bfd3bdf5fb0d6246c04899b97856564d49d693b2" },
                { "zh-TW", "f1ca51270fdbcd288b540103463d4ecadd86a39ca7c153cef27658ff329aaec4b40a4d527482047d9e7c37beadee5f782ca55abdb58ff7a7761c36130181e4f0" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.2.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "c509633009e43446b9c472d0f95f72b013436d27ae87f9e665255f957fa216b425e88b9cc5a67b40f80ef1afdb6451f67a52a5a107fd47de73d591194ff24797" },
                { "ar", "e590bd0ea8d6c08a1cbb8c959dcbfeaddf9e92df01ab3454fb8ec8f45a1c53dabbb687b6d520a429a9864b78a7b8ada1b0ecfd8b81f9c4c4bbe9c996a2f34184" },
                { "ast", "934d6493b1c5577a8ceaef2a181cbd9b7b8f8c5c4c9273623bf941a8e5babeebb9aea22e8704aa09dd321b4d8cafeadaa1bb754d65dc374e7657853305c09a44" },
                { "be", "7fde6b325635a36db55b1b3fdf359ece50bd8eaa56358c7fbc5cd25a930c1299615fc3c110069c453ccd8efcdb470996283219b35c4988b7ac54b4f0216facf5" },
                { "bg", "3d60f4213d4784eea3d0e7bc54d2af1744a6fa757fc699aa2c9efec9506a3bf2d4883f9a5586c4141fe4da6015e2cca982a7799281ae8ef016b8c507f8534b68" },
                { "br", "229fa0bdfdf68a6dbbfe8f8f1cda2d81cc1b87585610b3696c0e1dce1efcaefdd6cbdcad5b3bbe6f3d60aa2168fa1589a2ef5d5c691b1459b7a4510f1050c667" },
                { "ca", "be927c759db34de06f9cd1b50e1d4b6f39d507337a311b450aa4cd59412766c0a30dad3a75a65dac4a37547a2a4fbc639e24b0a6bd56ed363dee0d40b78f1bcf" },
                { "cak", "d71725c9ee2f9fec403817c28636d5f8c39255e534e655c0a1ecf13394efd7c3ac437cccfbd0a17ec6ceef4d0866dae4c6cb7f63295bbee948fff6c1ed044064" },
                { "cs", "c7d05a474dda03ebbf0b52c6922aa23ea0405f0fe5bef41d258109a75cc982f797135f479b0ca90c3a74891d095820797cda0630261ba727441782579becec4f" },
                { "cy", "9991adec71eececcce38a6255e1fa367b3f887ecdf20621b974598e476b2afd445b64cab22839def5cb57fc2757ab27f33353e5c4d82f5449721d75a9566416e" },
                { "da", "142885c86b68dc2b649602ac02dbaa3132a2ab0dc024e4e4fcdeaadd39ea8ed25772bd31bcf966056704f6620b7b8c37a8a88e578df562142e9acec121d07210" },
                { "de", "ce7feb3f6386e2c43bc7ddc8fc25d046f9616b6554e4deff0c2a9b8ba148701c77f2abc9d4e0be4bd95a32d6e5764b2e40be9a23c74d38faa60f3dfeee489042" },
                { "dsb", "779988d45524b50a2661348ed26b40b3d241980fe4c72670fee1fb9be828d4850062ca632273bde5c528afb2f4d1a9875ef2cafed9acc93917111d1745087412" },
                { "el", "4b3bab11f7ae4fd263eaa64cba14dbe9a7c6b6ad6f85785f23451326708ab63d8870bcb791e379d8b53b3977e7ef7f32db0f464d81607d62d491d122468aa2f0" },
                { "en-CA", "455dae2300ff661b12b91b0507584a857f243a63cd141a19aa12438254d6c5e2da727ef0d6967e0d8b0c576e49f545570c2e4d4bc15d6afaa086342ade991dd9" },
                { "en-GB", "045aad31b3d7e6de647f86d2c01031cc65237d07a5441ce00643213d2f91c918839783737bcf356692869c2b733fbb32d8ad5ead694948c362ec0313b809b19a" },
                { "en-US", "bb43dc83d7fb90ea6d42aa69dfbc34ee6cdbe90ee021501c1da2d73e3a4c442e938c0dbe49022b758b4764f35d965af40dfa995123c385ef4632aaf5491cab46" },
                { "es-AR", "d269882bfb5f1eddff41f458fa94c3c0f70531a341a3b32207535ab96741d0b529bc73bda69efce28c28a650842dbc481c77223bb104110f5bc7e2cd3f461942" },
                { "es-ES", "fd4e85e33056a15e2c5070ed77dd2d16f0d2c18398c174f5e0ee0319c10c3456b4db96e90d3ae2a610048999011c67c7ecbf0e682f6dfe393289ee1d26d06a02" },
                { "es-MX", "3c8f838f97b8599a50d59442cdb211455254defc6d80f21de1d9a7c23b3e419eba62b372d0700d8834b4d93eca23ff4c946abc20181d9c8ec8e8acf4efa8beec" },
                { "et", "472149fdfbfe5448364b5442fd9bd8b2981575f3e198cb25a8ac8a0c14879be9975028e27ce40fa2fc55db920e6fbb4b4f6ad8581e91892356cfcc5df4fbc59a" },
                { "eu", "b202cd01937e3718e9732d528ecbc50579640000208dfff2b747f27b0bf9e808b55f74b6524bb61715101bd30e0192ef7eb3ac28d9e2dd3e6454da8f4c888a1e" },
                { "fi", "e2e3edf81cef80fc10e76e714543469bb65ca7b2bbeb754c9ca87d2ab20f9ef777b0cba248461680eddd80017684cc46d97c420a40dbdfbde2147b6649385801" },
                { "fr", "6096b2d43913133fc366671e7b82133b47a043f0931bc3a51a683b2cc8383d59250fce8e085fd2376cc7009e72c18927ff9cb3b6291e38d8b601af2837eb6c70" },
                { "fy-NL", "508e187717eeb39b04f584ef9670e1de499b3b08a3709e552881365d6f4c014d0cdb973db2fcc90e8cf6dbee618a96faafd4fcbb8745d944d7cb1aa11f126a85" },
                { "ga-IE", "60da476be44bc04a4d32c1c02ce8a202f8b0b1296698c9f996053cbf1a35c8bc06af1df8be364c8e8886e17b71fad2c8ecf79d8352a6eaf480a20bdc732b6415" },
                { "gd", "4ccd1fab73b9d750b3674364deb60d314df89f9d2f9ac1e2eea1b8f4f82fc483847a9f970e9108823f93145d942a15fdffbad24322f154e12176fd5384b54fe0" },
                { "gl", "05a193f70ddac3311f07fe56cb8340ae7b01bb77b799d4aef8c75fc646bcf349ec7a1f87f124c9f66d45d2399b022cf3dd8274bd17e133fb91906e71098b627c" },
                { "he", "a5521e8714d8748727caa5f2dd26dffad95f26712a9f8acc9a864b63252123992a15ed04bd136b9f208963ad4fb57c153cdae55fbfaf47f97232ed35857487d2" },
                { "hr", "fdf7dd67ce5fb4cfad513011094847e40ca7aee15e20e9f3090512620c85d36bc96073e567934bd7c6828f8b6b2d70a09e8b013a2ed7e542ecce294b80ae6654" },
                { "hsb", "c6516ab278688f49858bb03014710547932cc69ed74187b5850ead94fa8ece55efb69a75b7108daf4db4e4b7e0d8dc39a7126a66ec3afdea6d9a964d3d6ac4d3" },
                { "hu", "3e2991672804da52b1d8e54890b89f8829cdb6ef6df57338329350ee06e64b3a3ea5eaa5c32a1a4fa8c59418ff7f8c22ae207a5f39dd8bcc1f7a0f610d03bc40" },
                { "hy-AM", "851d8b2e9933cae48908e7ac7361578ad97dbcf27ee50f77c7886309cceb822d9e3861f78e6bdb3b3e62aebe8bf75464704f43184429a8ed7b5597f0497cf861" },
                { "id", "b12b6299d8c97d3b8910d5a98aa1057d479068e4a81481bdb73440a2df3671f222ce3b33da3f46b79a7f77246c898f469565a3026248164b12750bbfa1fc9090" },
                { "is", "b7d130d40964664ff24771e0eb8089ecb4042d7cb4f2ba9dcd2a2751bc25cae0f4627a4378c3aa2b1ed46fef74793d90a68e04282368ce9d39613e1eb3aaf2f3" },
                { "it", "53babd21d9a855fcede84ce035e79551bbf072c09e2f9903b7e28c5fab3d5a0f78624d44e3edc41288b90bfbb999fa3ef9437eaca7f123fcdedda604e6d23ae5" },
                { "ja", "8e0e27764c758643af94c8b9c534366cfa783ea86a0711fd768717817a15ffd38ad134c894464dc9e06d43face6e52b4bd5f646035835dbd1c11ca3f1de37d9b" },
                { "ka", "b6f4a85cb38fc5f93e71517531642501ee450e441f99840a7a43d5ea048ad2d3c9c72efc8b9ab6ddc956f16b75e06477c8f116795fd8bb8473bf3d9299f08eb7" },
                { "kab", "4b35f74a96820f4b501487c6d7e487012789e576546d05cb4eba9d9726b4938948c70eae2fb19b66ca6127ab9c043ab292d3c2a26b0ad5983d8497399ba4c3ae" },
                { "kk", "e7437c2be01dcb756575e389a16081472edc6c962858de5721849e7f988d891bde14389669026e40ba002b37472224c08723177eefb085c77c73a4ac0b4379d3" },
                { "ko", "aeee6a2a7500cd137c9006504c400903a88cde872e20760f2efe96de1ea0e4db76bcc6f65cc27d20b7bef677df8cdc2e30d33a2496d13aabe76b4d822b007c86" },
                { "lt", "a09722beab9b3ed86556046599e622898a9fcb1ad6f0e832cd9b26c5b615089d1a8ddddf77e157770028621e121b99ab2b9f6cd9592770ac4afb57586fd9c3c5" },
                { "lv", "30cc7359d1a6c7125a6bead9c17469ff328f0f58c1f4f95b7f108819003ea03a2b3f0aa45461548799ffad502ad58433aecffd9e8020fcc849bd763829800f29" },
                { "ms", "60a82b6dc568e90c80c4b37df27263eee5c5e3663d2806d35c4729acb4ed034cb9ddb997b6debbeb5f8e638ce088e65114c8fdd220cf4c52cbcf645c3b8c00e5" },
                { "nb-NO", "8cc2e9e63c698475b36950bd43d6f197236ecbcc3717ada4747b89e7c8b26385a1867c847424c9b48bfe813da7a0c82c887acde2869b78c05f91c3d4fad7e9e5" },
                { "nl", "07287bc9f0b64e7080e891b9bd3167f95126872ab92a01806af864a89deef552fa94bfb81bc289d2f5271f26d807b2adf498be98ca7e4a47dbe21f5811a53840" },
                { "nn-NO", "ddee55aec6b868e9103a993336d6b51c737774dfcb875e3354536c7ccac67ab299c44750d14c3064295382047e28d75fefec097b9c0187dce974fd8d87c07725" },
                { "pa-IN", "f5af767dfcf65703fec04cd7d655ca797f4e5fea033ba78e2e3b2fc95078eebf4b84493704d212e9e317439e7547d57d2cc62a477307b8816209789da6a7b956" },
                { "pl", "23546451acd41247263aa9ba242b08f1b4f6deff49c033916bca1f0335998e253a6f99ac425f416f9fb04fde35cb94572c3bce6fa6c34b21f49e000b903556f0" },
                { "pt-BR", "5f7d02ea470db47c38b09584e1cc13ac219835158989c7409e077f671e7701d5f9e9d85ed330d97804894961438d8af51d1ab9702b7ccbc8184823917031b165" },
                { "pt-PT", "a1bf322cc9432dd09cb807e3944c6281359ec9f70d49a51f77223ae9736284993e49332088b31e5eb042c9748ad614931e92698922a2e57e3194559631f7f551" },
                { "rm", "928eaf5b6ed91d8723fe0168fe222b8d7a0b2ffa9372bd5d7330e649cba5098ab4cf684e32c5a8eeadb4723083b35f1400c77b1eb594ff466bb5fa384fe23457" },
                { "ro", "e07b152ebd9772f33cb709b3489a7bf4b35377d7fb46811656f2645f34a6427e38ca7b489af291694d4b2f2bdbb3d325e4d4161a486e6deb2dd946758de18cda" },
                { "ru", "37de147790f950c89ccbc7b9a0027f7fdb6d17da27cd93abcd8ecab0e85cf50a67f54faad5a10167bd88014054b920bb54a72e2be7a944595df05ee363c645d7" },
                { "sk", "9dc0a098ef630c0725b9203fb2109656a98c0ea74e49e58e47f7952e2b60e63fb7d0d9352cb2292cc25d8dafc81814aaf02827b8a5323d095c8cc115e5e25707" },
                { "sl", "ef4f36c83beb565c19fc6421452950fd00a963e59315753411686abdbe999b7f8a600f1db79b0c2d941f22b0569652b7a0d99eb46c3436299d337e23ce7db010" },
                { "sq", "853a26ff2539150aa36233a735ad2ec1d36b3a01b09ae4ce805203241f7be0f76e40fd99922f983bf5b155332e669902fc9eca0aa9bb285b63ef926cffc56ebb" },
                { "sr", "79e5ef5339a97842873371025d8804e405c98d73b96f1675d9aea2aa17f1df440df9f53ed347988ded558b2085b92a780ac9018e1a721ab603732fec5fe6ae1b" },
                { "sv-SE", "e10b2b579d80cf5c7565e239a61c130cf7d1c5ead874289072759b6e75fa4e2e6aa8049e0a509ebf8edb5de9058c5f5e79f7e3878b50ae90c5254af43f18deb8" },
                { "th", "69fe54ae7ec565a0c6e783878692d782a9e715c947dcf659dd3147304c91da7f2e84773fe0377be9e607f6b9aeb7636863accd82127d160212c8fc6cdf09120f" },
                { "tr", "59f68565b939fdb1bfa761673420dddb4573258f2e6531f556a1eaa137b2025c4ca808cc8c8cf3654f6d80efc6c3d7b5e078614262330779a98070c67fa37b34" },
                { "uk", "6b7ea4ae0730baac5d2d8f38c498693ca6a84300eb72f0c6cb5f3eab411c246e4007cb7d900996b9de70464d058a14cdf59acb6ab8721b97846ef597ea62268d" },
                { "uz", "9037c7152d4b39fe54fc11ef8c2531e30b1b14a03060cd75aeed0e83740567eb9897c2bbba9451cb022e4cdd1ae858fd2aa55d85aa6d123c0b0cf59af6794fbe" },
                { "vi", "b05cf8b62442d9bb2ad02d86cc086c4cdee773205db30e6dd1000bb3271a0808574d5b5e2ea1eaad275120686ecfd970da2a2e4835f3e8d8c6f366ff45df05a8" },
                { "zh-CN", "f99df4e59ba8b25a85e0f054ddc716601cb991791b9c29a268ab615568eb027d02b4878111a5d1f20c97f10e73c573d92a6b0bef19f78554579173dfa3568781" },
                { "zh-TW", "f4e4ef36e6896c6de1b3e50c8119680f521d7efcdc59a226eaa59abaeacc05d059ffe225b54bf19c66cbf5482e207b9b9d33ad1c0995fd5312b33eae83bcad09" }
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
            const string version = "102.2.1";
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
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
