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
        private const string currentVersion = "123.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "b364d0b5ba1e670888ea01256ade60f0984de02da293bbafdd91ac0ef3f5c2437272841bd34559db28c5e0d337f059099c844d7a9454f9561060d5913641aa74" },
                { "af", "b6790176c57b500125a0fa1c54ea9f6aba2831513e9a7155aa9baabcf399da84ab0d6fd80a05826a6685f3a2c91603b2c952f8ae786b3cdd2a8360f08fbdf9d7" },
                { "an", "6889c3f8b96efe94bf5213b6b6f2cc78028d16918dc09ba1e235b5a383286a598ba634a83ef239d7df41666c7565f7628b71eba6a86cacd71454751ab3b09ab1" },
                { "ar", "2dc984eb91fc9cfd0efee4c031295475c625d0adc7babe0d455b5f6955fef27cd0f8c892ae9cc310df14c3e55067b6b19caccbe96e8062c67de408ede233936d" },
                { "ast", "930c73e435a573bfe3cf128b3a51b05dea36bc684703b80b9784c22f64253e2cff98deacfc6a871c95699670dc199f914078c60abb7ffeec474ed34f9153e17d" },
                { "az", "c4dfa302f16f10e67d22a027a037b884463a7cfbfd554d3611466b3270ff6e6b5f3fae4fb13b33362f52961a21a16d54c70514f2090500c2348c60ae421a94f5" },
                { "be", "932307d6c72d982d5e9ed594287d8b6d6940b1ea2413c17f3231426fc2c442884c01ead94ad84d476319ee1390f83e034c1064b0b663bc4613618f40c6c789cc" },
                { "bg", "daf55bd609f6ef3998376bb83ad026463b6b213402ce76e367fdbcbacb4083d994c976fc7ce6552cab91ebaa553f99662869d98192eccbc9035a8fbb2da6c64e" },
                { "bn", "f0b4d7f9a6e0b9a0e7c66e616ba6d02d0954d98570f1aff396e0b42b4781e2d6eb460e4eecbd76f1b4c80b65e8137731dab3b3636a4981a218b90c0b3b63f2ad" },
                { "br", "bba6791fc915977dbc5c53bfa4831466ec8dc9704becd1f49722563054b337f091a02d391431d66232c485fdd611b6015b58398a12fb777a7a02915bdabb339c" },
                { "bs", "2adaad7a57360d413315071077ba914a1157829939289b950f71e1792cd0f4c11fc5f1c6fcb033e24d5eedcc7f0ad9403e4825e674aa5bcc9497b3786c8d8ff5" },
                { "ca", "15b269c9b1bab6bece77712b64092f2a3a474b2a25082165e5d6626c23992b61ff0eae10aee78da078c94fc9704b95b904f26b94fa9293e9fc74ad1b1fdf5696" },
                { "cak", "58b008893d65ccaa77fe90c2761debd946f232e37f78b986050671f4d01c0c10ceea1719c20e43820b245ba4c890067add1bcc0118a9f03ef9732aa2cafdd614" },
                { "cs", "7d4e0ca64d8866b1e0b71dc8add2a1fbb194ac25cebbdbe47fadfd30113e240e7cde53c57145ac90bf654401dabf558294165312b1acb8a09e0389d0491d45fe" },
                { "cy", "489c7cc236be272b8e84b5ee77aaec88570d1fb7bc1ba041b9ac858d4face09259d6f09f69373aef79dbd62f731f8e7cb9ba470b3d95136316586f9113023d8e" },
                { "da", "f62241052d91fdf846f5a34b1a9a00d325f5651d402f1c3bc64628d79b5a477d9b28a8ddb4371c4d2ddde27bbdb111ccfc4d8d798e13dab78211145fb055a407" },
                { "de", "f8651c94d9699c7600b3b978cd1582459d6dd914788efe2a105d05413e89463c4b880c6d4bba41994e149459d920658b1b60ad009fdb7c162fa74e6fe0df6a71" },
                { "dsb", "751483b03a65a36aedb4158009a76aa7ec3d8dcd013341a460d5645883e5c3a11c47a6e7fbaedb9064c7e16c76e380d847903122ff31d895a933c1c9f0edc700" },
                { "el", "5dad1a9f592b1a1e68f0071fe6489f2c8549413323b480d8374c72bae3491f945b06ee40ce98ffefc443122e45ddaacb50f50c521725cc2d3a769d69a5e9c698" },
                { "en-CA", "8befef30a5b5a160b22cb14e10106be2d914f36b2926cfaa9aaa91a1878f1eef0031804e0ad5e7103cf2d67e7bbc0d5cccfdf519b002a03f6ad3202889768cee" },
                { "en-GB", "458b62c70f8c5099e1258c6a3ff04fed24599b6cfa609dd0466f101ecbd3f21e8ad70ceb72278886146a64134c00c1df49e3f4ede1b75dec3ea7216a6cabb18b" },
                { "en-US", "aaaa7dc125cae5455b60e11cfd7e9756deab2564750ae3b651e2a5100edae50a714d5442a4018f10c74f5a744a9c28e4bc2b0b6577bc6e98902871dc05b79583" },
                { "eo", "0ed4f85ac02d61c9365da080741724505a0b516a98b1e64a533204b930b14e740c0ebea80254213a542888dee1b67ab3f482bb957ecb852810f3f1a30c46f6f9" },
                { "es-AR", "8b5eaca706388921beb65306f67fa3664e4c77c6f8ce1dd113a43543918e096215ee531b845f9c2e4d8d397a896d8de4a58d6d8a4386d54e8a9be2cc5550122c" },
                { "es-CL", "b9c289599887eeef1320b04f3425825b9b55b70b9a18ae5b70f5e977ad860011999942202992598535f046057299f15c8e1af76e8cf5c0fb811dfb2f05406557" },
                { "es-ES", "3c7ae94175ff8ed9db1f9f4bfe2a1a7f83309f857e6000a929fa3e844a1f522c5963885aa10715bfe045f55daab76a74127785a232edff0e90b53de3c58dca27" },
                { "es-MX", "48f0ebd6924dfb8d0e69e28ed086b2220c4cd0915e4587cc8826771fd39d89883d01d9b276a4c571ca57d9ff8bec312e01742d5ed61d41788002fb16bbd7ea85" },
                { "et", "37f536e3281f1824e7455624703e3f5f0a8742ece51a4aee28940e7556d108fe0ff6979fca67502330220ff83418f47ad580b7bdc6103b9c6f26bf91cc84f446" },
                { "eu", "3088fb0e2ad91f4f49f9112842e8cb2920db651e1e7d9e87c17592030c232feabd44c26cf895e9a88c740dca34e0e85174228d3036795233cd3c88bc56d6dae0" },
                { "fa", "6415cd3d3837409256b44124392a547cca5595ff31348cbd1d42aa24a5ef5bf6d4aa283f443a5e3d2b0add99ad98c84069e216b02cafd15a8dc58e3e4d782e1d" },
                { "ff", "2979e6a5a90f31cc2b4c170d4c9c82eaad7d738b094d8ebd4e8986f7736daf41fe7a5eda943bceed5ac2212ae123c2f58f7dfadff2107e37b531edab9602cc57" },
                { "fi", "deb57c86aa48b4e269d89f007d4f927c7535367e714c2a9e3c3a86b710a4924c2a4e9f083ea703f74bbd1cee013874c2bb50ef852322477b410abf2f64263e73" },
                { "fr", "c481938b1ee54c2e1cb48ec76b92ee247628f23eedb66f44990e77721e46d5e5378fe09a0249095a7e69bb995bd6ed2a937c8fd70e281f6e785d39327ad6e0a3" },
                { "fur", "ea973f293346623ba58e495fc6ffbf7c3fd7ba9fab31002cbd59aa16613b48584f17756654f8d7bcd50eb4ead84fa48d674d00be12f6ab12e291db20e83f95cf" },
                { "fy-NL", "0e564ddc6a6e0fcd0de618c110c18b34f89d8fed5f4f473000fd6b7bd9ae16c06933f643fc951eeaad12699a54bd1fae40c2f260300be62eca4dff4673081fe6" },
                { "ga-IE", "add3a9e9403f58e85c74d83eb3dd36da024f7e30a87c005f3fb3f0da6878fd49f074e0b3f113b43079146034987a06765c86fe0131e51c4e6b1a62e268f91cf1" },
                { "gd", "ad4d3d70fad098c5b894645bbe4a842b8d7434d0878a0fbc81eee2fbc7527169fd83afae8f7fbb619998a90aa841dc3d2e930830093c2150ff73fca400010612" },
                { "gl", "17618680762b670ea18a7790460e9cc00bd80ad2e9809d47b71a8d54b7bf48a6bcd0fcc83d6e6343dd6bb9dbba7407c60faa548457d69e8b9752b0539ffdc614" },
                { "gn", "3117ab5576faf959571c6dcdcc06ae8e7fafef71e9b00967b6d24dc1628d112e5a3bc4d45a1f1e5ab4654b3748908ba4e4bf07206e1dbb8caa38eaffadf6ebc4" },
                { "gu-IN", "27c2bfc1eb1bdc950aeac8c64cbd7af80f8eff2c1c31fa7b95bd45e06f4645a3769586bdc63308647a51f3e085bd93957b6629f435a580addefd46cfcae9d545" },
                { "he", "b198e4eb03adeed2225435fc514c215e26dd0c15f1528a0aa15302cda9ff33cbd183a52a0128c2c36612c5c28dbd2ecc09ff4b659206aa95d5b8d347cd3c0558" },
                { "hi-IN", "50ae8d53fce5b1d801ad81feaf6203d172e26b7603a86ba31684821997dfd219d7ca1093fae4a78a6a706dbcab9a90f60fc3ad8ccee25c35478216f59ca533ea" },
                { "hr", "30df41208817f32d56a3c0be16114ca5954c37294020d4e09249d071fdaf5d37f966147a8f89373a24ae419a469fda8d17f3de68603a5ee2b8d75b32e436adbe" },
                { "hsb", "0186303ecb5a2c970212c70900471d4a1ab6441e17ba00f49cbcdc3a9373760ed10177719e8edf7c301274fe96a21f5b655349f83e7ea0c83f3a19b676cd8acb" },
                { "hu", "571693ce17f664abe2d8fe29b31ad6100df6a4ab79ca90443e249fbbe8a811170f98a42a03ba030c46dfdb5f4910129391aa7eed2adb555f7171f02d39da2332" },
                { "hy-AM", "eb80192f5050531f2229ad169040ea0ba6d38c8a887f3a29ba1f1aab252f5fc8642b8f46a3ee624582abc11e312487baffb1b8274801259dd21d1a925476d317" },
                { "ia", "cff81d2da594361e4e21fb44011d3ea4bf62e1a101451d16123785e915008e0c3a0a0449b7ed0e7b00c580e6b0d3e0aa286b30c4ab1143c5900253d0ed601702" },
                { "id", "136b76e97d829bcbf94673e68b018827d1b809d9037016f426d0223af49b66728adf61e492a8477abcb5c48660b312d086965036e9efcfcd088b30afe5385c0a" },
                { "is", "1525c1aade1d805ace0940fc8f5ecc7a30436dafb14b31d8f087db71f046bb1e66a839ace69192d5dbf525e44633b94760f09be978d0f0505d6c7d6c6a91f098" },
                { "it", "cbe73e0c75fc94981344caf9dcb7a489d5f42ccca88c8346ee1ab1de53b9ddc28b74410e8323f8533aaee455afaca52a07a0f7ac253082a7a5b4a137fb221a40" },
                { "ja", "be9f350c867c9cb3bf8b78446a1f44754ab4d654748505262d62e8eadb3c0a7f5cba67f1276da0b74022fee34a2e292ec767539b9ef10f027bfcbc5857b9d401" },
                { "ka", "b8ed3b52e25ee3f7572466d9ac7f4fe1d329dbb5d38ac1b0e8c739d48fc5053f0bad5801051f68c12d2c9af7d209b3f4a9239eaa9aeee402b712e48ea7aa0977" },
                { "kab", "ce5cacd8a4806780291de881730360a44c5966f01cc6cfdc17443e7d4a0f828de8a39076f7625e14858b9f5daac85d5f5c684c9217142434496060c0d53c4c65" },
                { "kk", "baa7f6d7a60cc3d2b655fd16d0720c0a90e2f2ec2d175d847d27e07af028ed3b90c48fbb15694b01bee1fbeff1f9662cc340c5a5da6bf12ee735ab5ca5402c9f" },
                { "km", "e8f21352fe4f7a0359723449d81e10c03181754d872fe0246ecc8e6bc399b790bbe2ce82a6ef5517a50d2b12cfa92b327759a8472ca437558f154eeb328e92f7" },
                { "kn", "33dc1743352bad90f64563a34e54d880bb947788e7802b59d45e9c6fcae6ebd4d5affabdc1f6acbdb1c8ab52c7ad890b292b3e8b74a486ffbe56fa4da2e7146b" },
                { "ko", "09e4eca3ccae0ee4526029fa0afe70490b0ffbc285cd1947ea4e38cd8fc10893080f8931631750b708431f5206a2724ac957689812b02c885c430624fff05a20" },
                { "lij", "46ca483f3d81195fd302e7636ccb90cfe7f3e123daee6859c8b333e41bae1f839c9ce7a3e07b00147cc9610d6db1c1130f4fccb9a17d1feba0fc77eefaa9bcc1" },
                { "lt", "9b8c132456ee47bd6cda5652c953b6dcb01ca1e75bbdffef385cea9b2d0388d62a5e88545a005ca77f4c4811a2e233885d1e38f869ba9ad2eb84a6fca7316eae" },
                { "lv", "05134c8352f5545f77a2a5e3d336314d84ab607a8bbecf01ee86dce24f89a00a03ac84cb79e5177c5eb033543ea1fb72514231b0ce372f1cb473f537e93db9b9" },
                { "mk", "9bb0a0d760678c6360dcb1934ddc479b7b30cdb5f85b1e55c647a914d1b3e6d4622d55c6c27484d4e37f84d22c9ba002cd6c0b0353ac3c3a1b4dbac437234c37" },
                { "mr", "3fc4d1f3248b502fc21cf439d46ec8cc9f56657e0036fe59eab1fa8764b5740bd4020ce692e2c5cc30d0ca2751bcccb521b10fe383efd07435da84500e4ba4e3" },
                { "ms", "c5d7e46d612d79986f697904de4e4f1306ceb46be9df75b95ee87885a7f47d65ea892452c7942e05ae71b2809fb1a53dcb1991ff6a4e9fe7cc429d193ee4d243" },
                { "my", "b5ef0dade6bc9f2087936ebc9f97402152e1ad77e32e90f76767df0d595c6497346615adf99f5030b4b21d9a63115b1450b7948bd174418e078e949f554534e7" },
                { "nb-NO", "040795c0d89ee2153cac60b2c00365799869539d5eff20671e55945fdeac01a8573316603e31a2d5dc56ddf30ee4d3224b5b2758affa2629f9568294b07753d6" },
                { "ne-NP", "10f830de540035c8f716237454cc6877b7bd7a3e85c66eb9c7128c8012608ffc57b9954136d845850d147c4bbe52f2e0a3711ac44cbb4b625e43b70fc22110eb" },
                { "nl", "6b7171b438a12b0935e63f6210e659a9676b55e1d8dcd87001c7ae569f3afd78d0ab4cb0844565c653827d8c537b870205a021fd92903bc55ae3b00c97d43d2e" },
                { "nn-NO", "742aa5f72a31ca10bae42026b1d18c5bb16fc5e106986cf8abd29ff942cd2405c4fabf89a96a7e5971fd703f2e51041bf4be055f54968f14059d025877a033af" },
                { "oc", "bc0b2d58c203e6dfe7937584640c7ee27bcf0d81ee83eaf396fb80caf4498fbfa576581761d234af7cafeab92a49ce1515a4e6c84ba25728d60dc85226532b87" },
                { "pa-IN", "b2a74cabfc701361f82c36a6082ed362340bb22a2612104522b730688c50427cddfa0e447f09ededac0c43143660fd22020c7f05c9a8310c89065e09cbcede12" },
                { "pl", "d5d0835c3581e15f808a3168189063f5f28b27b80354ae1d49e57690e82624f45b7ed0caa08c82cb803bb14089495d4c48107cef6c1199fff6d2fcd603048756" },
                { "pt-BR", "fd9c752fec4584b774c9b52f9c631d6ea288a877522b22a2c2f3a9d88463d2e15fb96b814cfea2321f097c92ea10d59d6046d8b718435954fced5f877f9b7fac" },
                { "pt-PT", "33002261bc2b24caf53216d0dbb505f2454814f319689a13dc80e397258248934194be0e871886c1202c0ea263a2c3c5a5cca9a371cba28dc6a0ae7aedd8eb9a" },
                { "rm", "3f4847b5d24c53746b968fdda3a9b2ef37ed49f4d2e0036a90f7a3c68fcc71f4f480a65efe9c47a1ae96fd9ba3aaafea14dacda895277e70f8338ca8b6ef045a" },
                { "ro", "9c35ed910f5403ccfc9da633997b9bc555891aaf8f988e6c0aea9a379b311103bd74e75397bb050312e19c3f6211c22b09bb4f3176399676fbe8f4777d5cd5ba" },
                { "ru", "41f32ee334a9755c6990adb526c9b59291eb20eb25e66543e3457be02d4b12f003785963f7f8ec1529021b60e32c9b5bcbf6a1ad0cca1ec0e9676e9b817bf953" },
                { "sat", "90f61036874dc9e09ddf61ba4fa88793c2087f80d0284a8e857cfb0a2c4ad44f85feb3b009c8ab1ff93afa5963b0d965fb2ca59c2c5f2c89403b76a97e93a6f7" },
                { "sc", "3a5a23685f4b80c07c150a331f7b7edf1725ffc0a1460fb51d862194ffb2603b12b89756429105fba00c72cb51cee0906492a34ca3a6b7e69b569508149e8028" },
                { "sco", "68e491ac08e811e73419f77ca804c528520fcc704e5059cd3bd506f2f275fbd74c04c0926a466ff4218b0a5ea9fea06c0262274819423d4e7dd5b556218cf8dc" },
                { "si", "b4a73021324c5c86f7382e5a5520a7b9681d8c49370675cfe6e299eb63b5abb17ed5610cc2e85c06e11182192023b79156dadf7a3c336fb598f9e652caa608f1" },
                { "sk", "047b48d1f586ac6ebaa52810af4cdc5cf5bb86ddd68b26340696511c4ee119c28a54e82e99b89279a849e9bcec840380d186e25d4435f3e4bbb08ac294835089" },
                { "sl", "60c7c6be9a59f73fe3068a3dba4d8c261c165dd2870b89bebf4166faa30cd42a782405bd37227e576ac3cd3b5f5e50470fa6e09329a57c340fd73fde43d317d3" },
                { "son", "ff620252c6a241e0c3edd3c866653dc8e72646068ea92d77c21fe316b29c93736cf16f9c72ed1bb5fc082a1ee7055f76e1a52e1f05fdab1fef391b2107f10eea" },
                { "sq", "1dbad16f2528fd8f32a27e3b1ceeeb13b9817903c81ad677cd990e7bd3c782c4db596d3f5eab22a233738580147c4333b53d8205259709d5e98c20a91929b758" },
                { "sr", "b378be8d47ad6a2d0b156fff437919b465734358a179eb50fc987922c60e41fee6e19188475724d988ae55a446579880659508ede837aee1557d2ac776fa5266" },
                { "sv-SE", "37a9a086f991c0eeb2058b00a9ef8d41a14ad3ed6dd9e467e6c3421fe5afa46fc78d0a3c6a83faa515ca9173c358eaafa883666087ffe3ed64c41b9faae02f11" },
                { "szl", "f98674d78866924229b47eb774e2fb8d0deeb883e880bc8478a6ef83500a3d9a25de658c8ec45cebe8c84f2f39c58d067baacf3e103a7914164a35b38c74c871" },
                { "ta", "a970fdedee4f9d321d1ae0577e863033f15f736b0d25f05b62dcc05a4d9ffbe1f1eef01cfb888553e1be2f8ca608e64658ed7348d8aa7c099f3db22c48e8dd60" },
                { "te", "4eb98d7233e0d240d15d9331956cbd4ac254c17ca66c834f80abcf95711d26586aa1bcbe84dd9aeeb29ae6d6d09c06adf0d55f0ce1acb56d86eb9cefb246c4cb" },
                { "tg", "9d034aafa1b32c8e5f076074e92d719477b4078e4f5fc32bb3d86bf0606e6837495d4e6978c3981f63151c4ee8eb69e289e70821ff0d27d997f8a9cf7826fbb5" },
                { "th", "c30d58279326bc2b938cd773709f8bc250da334c18126da33a7907bc370979256b0c9159d0490a8e0cd0192b693c2e973025171ca371fd14464e35a6257d48df" },
                { "tl", "626b56dcfc4069a78a348e0e21952b67eb853485fa837693c2565ffd16750402478869f98eceb482405585e74097e7dafdb0972811ac98c2ba693ae551c21f79" },
                { "tr", "c763a43e50dfdddbb1d76828a11eb5a67a4592bb81dd8fa2ac22e537900714b4ea1fee097e700cfeb9a35edc8c6529cf19faa8a842ff2be1aab1c2c181384c0d" },
                { "trs", "2b8ca28528acb254e90dc5ac390b687b332ecf114cf2a7cf23c0407702a1b42df6d58268849bfa34b281d23c9ea50cd431e046d990653d84a8b85e779ac23654" },
                { "uk", "8596ebf615c77fd613552d65701af7e839c650eb71008449deb51bee1330a3ddb2cad8d5fe955fc8698d67dcbfd388728b762d837a621c29ade77e683104aa81" },
                { "ur", "59bf70285d0ba87c73deca2b47104c1487f7617d6e5a4cc1e1605e8201069c8ab3a66d1d413df98374c1528d34f39156eac5821d653c29868291b90bde4a54be" },
                { "uz", "4e8bb4c07fd7749b06759cabade99f200b47618d817923efd7156e4ae4e754f5a6bfcb7b6e0f15ec79bd2a49416bd45059b425190255b8c921de6041635413f9" },
                { "vi", "d62e161d5ae3d1a75d2397746944316db716b6c5899c1e5d7bee67b21dde376576adfb191d94c37e2bd74072f6634ee8a62ac3e7082738755722510d1264c108" },
                { "xh", "fb806da9d644bacd59f8ef7e1a8567bea9f42ec85aef8424f2a764f0dca33be824bf4501231641ac3291dd86f5284966746c9c48898c21cf0bf0208849a735d3" },
                { "zh-CN", "f398ec57932110175e211931f80f03fac536df422fcac6fc571786c1d54529c10ee4b2999ca45c6551d80414b9cea8242d42e51bd4da25e171be6d9e233ac416" },
                { "zh-TW", "3e7d31f43011fa77f74ac1e2c527669d91b57e41b7354b451317b1a39a38119d425513227161a1da1113c85df750572ffa50fa66e1e2a10199acd45679556812" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/123.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f0e2dc68417011721caff942bb67e4f709a47141403f2ddcdad1c72d8a33123137cca11bd71beccc43d2016a9441c18f669b33841719de066ec18563deb14128" },
                { "af", "43c72d7f1e2f2f0e196526983786fbcc16a6512aeaccfa15142e33215bc8fd5953b32116e729fafd5fa5a8cae9e8ad4965dddf300c12eda04cf8e62cdd02cb6d" },
                { "an", "e6bb60444aa8cea6012794243d805661c82b43081a46dc1901b02343a4100a4a2caa4bab9cdafb4a6cc359f0de101aac898de3cdc6645af4f2baeb03490d8e95" },
                { "ar", "fab1b933b891a27d29931d645be689d7bb5ef3c65e389fe6aa602af16b0db52c1a43839188178f7ebb2b738bd6be8271fc6c8e8ba729e785e7a57a897705cfc4" },
                { "ast", "64e50aba7bc0139cd3a771f2249a04fb2c5497fbf0d58a818a94d0fdc068b6ba21fd9eae008d4329b78138791a192f870d034ea58a0fb48a0cac3f69b95c748a" },
                { "az", "7d95b34ef6b8c0771cd142d2a92cbdd43b70ba9483f84c14244d3dab4407764a7285d82b25d60117af4e79b1b15b61ee9a0ab8f945945bf2a4291aee215c00a2" },
                { "be", "29230e024a71183ecb78e053d9eac416c23148c7f2d77f5093ad982d96a2e02e11cce6f292faafe303fe818a59ac886081db7cc6b43a8d0918fee75de6e53a0b" },
                { "bg", "a1d82322d989d8c14ba9a89ab2c7cfeabbadfdffad9350a4497f4ddc65ff15b3baeea26a067570fc1cde4354814bc6bea9063b1dc8e2d4e1a71721b075182238" },
                { "bn", "f8bd8b23ce696f96bbdb63e8553c1452eaf6470056b462856c056d0c5ac0a7424de9c5285095f2002acc5f1cb9a550b88a16bfe4564611dc5f1bd75f4b49333e" },
                { "br", "20ed88e4924a4cf5e0f4dec5e422b415dff5845893aa703fe10a19c5f41e58cc4d70c4ec35faf9c310c9a3e39cf47d19c62e017a99121fde06127c869c8d78cc" },
                { "bs", "8dcc247776bf98604e0673e6f14074ab87c1bd539295b5127fc85e05baa3240b5e5344ee4c05b43e4a450e5af99fcc056201c1c4550ed66f39995d20c5cab3ee" },
                { "ca", "ae5b3f7a2e9993c23a260825edb55cc1ce6da12f565d345e78ecbece75c899f2e6d56f3eb59d2c1e8c84c61f54373617b4efea92fa0430078f106d9e51872296" },
                { "cak", "a27f3a28d34e97bc264df4b1fcfcb78fcecdc6df5402e6447f4b087f5e942a8aa304fb1dddff23fef17a7fb8a30d04b78f23c729fcbbd16d168e26d7acdf6046" },
                { "cs", "444db254827c11b3a7a6f1a3e9f41ff302f8c5c70ccbb4c2e1fc47719bf623a7571150346a9ae18952a86d1a5125f80037c1fc4cb09f3c8d888d7330a7cbfb3e" },
                { "cy", "5299a4319e6200bf15eec369ccbc201f4998e69d22a811a844f370c43af775a29c993c37467563f8570b5c6c8823b691225f6620d2cc6f417f716b35379caf52" },
                { "da", "a6927e00a38f2b907cd3fbade3ed831a415464623a19de2f85127568ead488b25435920f611c37fef8398a587d9af2960481b172adb220044accf92164c4ba7c" },
                { "de", "d2d9fd97efe8290f572b5232735e12860c34a5e734d0340a0e0cff8d0143a4a515019ab7539816c2cb7ee594b7e14662e036c1bfe05fbc092b8bd361763d02ea" },
                { "dsb", "28ee8fafde694cfc4f77115145b7cc82f280de2c3586d70638943e9c8e176626f613c7d97d4788032ff5c9a0aea69c26e4139e6d0cae3612db0ae56911beb885" },
                { "el", "bc5ea0ea3bda1db385497b5645b87c7a11a9aa882ece24517339f865a36c51c83abac020d6a32078891eddd0130670168d4e80537a12eb3fb1100edb1158c705" },
                { "en-CA", "5d9ac71e3afbb7029513d052f1ee0f76dc1f052cf43985d32ec85630820b9dbf1374ae92711dc4c1580202f1e9d2b3657fe733574f9f712ce81096fb68adea2c" },
                { "en-GB", "911aef0cb6c7627c548d4f00859fdd8c4eab956df4bbe41e4a1fd3ffa96d1d1e4becbb76f9c23739d47220fc7b6d58efe27fb802dd9209c4b435156cad07f22c" },
                { "en-US", "2627c6575481fef7763cce8dd26fe9b8c4711c466f7e6e33df3bebb885207ad544648b13b747e234b11ff2d2b7859fc99090a3ea34ba6940c0a776bcffe5a048" },
                { "eo", "36aabb1bb6d16b0ea73556fdeeaf9e7c29a9cf51053a3815b24907ed7ab24ff62445a1ed09c232c5b5d15881959ce367bb9906d54b4a3ac25643096c06bec0ce" },
                { "es-AR", "1ee1706e27b3bfc4b3eded1aef19a76f930b3abbcfc8e5475e6ffe0b7d9f4e1b1eeb7d14d4d98431cdb73f2900de2813ae0d3ae29df1fd7125b6c43178cfc0a2" },
                { "es-CL", "2662c0d431b496940e4b87a460d39960228b0f7329bfbc04601984558e8747be4b450aeabbd3489e74679a30d74c2788c02175154ef69407c14ed7b603c02ef4" },
                { "es-ES", "51a34a0fef55e694789db75b7841ac349672e8942a85cc70da046d8b35bae034a75c5e077e41ee2898f9ddeb744455f767f0206c1a22fe6b3ce4839cb9a0d334" },
                { "es-MX", "e9383e7c5c4536190a075b2e8fb9a3e8f3e666facbe1d24c92c605f9ee5bd474fc7fcc1e2de40905f211e164d6ca2d2558a59f2ed43713374757ec77c906ba0d" },
                { "et", "7e9a5b70ffb8a7dd693c30fac1a409da47fb2cf939e47e83cdd4e8e08f18586045423f072365c663f033ec7ba07a367e80f9268711cab89cc353867355fa0987" },
                { "eu", "83ba35aa62ecbe0cf6f2f64701e57db7ee07e395b3f24c98849ad180feeca4da11eb2a1d7dc519bd87dc63db6f76483e7df93273528b042a54d6ab4f48c9b5f2" },
                { "fa", "12c5c92391069a070eb33ec67d5e5372bacab5238f654874389d2ce721350570ecd49cb341c5b757da1a5a23fcb50c9e982cc27d2e0f707bb6eb7037062d06fd" },
                { "ff", "5b1e1c7539f06b0c428e9a5910af97a54bbc73d5147511bc1042df8472472caeb64c5658f56f8a430f1903260053cd7dc28563125fd67626a91bbe679bab6248" },
                { "fi", "0d0297efb97b712a9a17a69c37abd99afafd949d9182265f86bf8525ea567c1ea9d359f57beeb2601d7d0ddcd73161d7fa4a4d6a357b60046395593d2fb826d6" },
                { "fr", "dc2d979e4b2af68be374f9ec9837f440e09f936ecf53733478df3648e863463c74a01d2515e8deb675f49b9f30aaeb62b41cbede3cdf3f80daca3e80e88e8fd8" },
                { "fur", "70d55b0fef0560459b267ad1569f55eda50615588da78cb8d0fa119295eee8fc8303fc4bce8fada9a0c85c00bffbd44d65c46d9029d9c7d9c99e1e55ffbe9a5d" },
                { "fy-NL", "bfb147dd7ab398f0ffe324f8d694147d6ac3a2ca9a287e7c89f0f18b227a8f043e147891ff4299a26d67b106962894347c1080f3cd905e598aea29afe9c9d68c" },
                { "ga-IE", "e6ea2b5db79fe01479df00fb02fe82458010b8b035c1b20db4f0957f1c9906e4f93dc81d354a0109af91a8b65cd4a331d37fa2cf023c1a3aa5af208e07d6bc69" },
                { "gd", "c5a777f62678318de26a5e14f63a4faea66227cebc959588a7fc7f921a72cb0f90c2bd5d1d44b79f96248451c19062a7eaef7ac9e2db4a9b88723e89571b5b91" },
                { "gl", "1bcf8775d68f26d3e9983da3fce45138b5c9bd266235d4622bf53d401e71567e348a24d5060d2a9278c603981338f867fa7b698f49fdd34269a22cbb966b7c54" },
                { "gn", "02c9e3ef66f9d3cd9d96ddc8f0e0003af07f02f36dbe85b33b46e70683f2ae5714fb074262abacf034471b65b05a07fd6a41669915268fcf5832356a917167dc" },
                { "gu-IN", "92ffdead8b3b39576e882a65df7df67527a0304f1e55f8dce2098710d9f7d2e0f7d1bd0694c9ec731a67cb9fa21b334f7da876f9d8cc39a59919f571d0dae87a" },
                { "he", "3d78d92a7a96c9b81ad6500334851a55f3dce0f7ff6cc9a8a49f37db5ed0b7a4d8680bbd3200374a962341d9480759ea100f95346b46f422fc9eacadbd1ee394" },
                { "hi-IN", "944215c0b569f82c4f541ecd09a806b8d044141b3702e546b7198818ddd8f73a95eef621ac3d5d1bc6ba5b6bbfed34064bae46fda8fc8614780f62a9e078fff3" },
                { "hr", "004e7015a5dcef9cf4430d6256a9cc4924823e93e611b8c014835c1f36287196bb55347cad7ea3acf3b716234b26bdacae7297519f8659cb13971e7bfdf9671b" },
                { "hsb", "d5c0491070ea3d0a1d434b86ec8051f66499423accd89c0a855759968063cd0b139c99a2985f0aebd7f9a754e51b7b26348ca62fc3a6db322da6248c24e9e0d4" },
                { "hu", "70bf1308892396dd4bb0007419f2c01caecc7eea1cdef73d220e81392e6830f314fcf114afe7226b711ce4710795b61f1ebf900e33615afeae1143b7e29dcdbf" },
                { "hy-AM", "80149eb89ec8cb211ad1d258a53afe77267e491347987e4b0da49551a0813418d06e679ee1a84b6af8b3fa7c3f57a21ba8d393be7964ed1490a300a02511b1b1" },
                { "ia", "88bce02c40c5c9f17b60fffa6a6b0709d89d497179de601c8ca5b05f73c0f4dc895c7cd374deaa0cb54c1f5424d4fd934590664deaddde96f9a36b342e722c1f" },
                { "id", "72669b0504fba3354e719ddba9a083f6e400cf3b3d626c356c39197c8052369a5d5c0e5f510cabaded148703536a6cdae8963e25141ee9df43cace62335a7659" },
                { "is", "a7de62aed7a74d092b5d9d52372b7234f39c813d5e45996c7b897d84e0af3375b508cf32a35e78aff78b75cef5f5469ad294eb9f44df46fb7d1dea06baaee2d6" },
                { "it", "ae7b9eda3547e21f0fc74cbbbceb7766f6c28563dc47cbe94f9c308c9538999069174059d5b0cc01e52f65f3085e9bc4564c6dff3fae0c5fb6f42e0561963fa5" },
                { "ja", "75555876471b110d0c265b84d0b41057c5a305484d435506d069690c2fbdeca02ae8938bec0cb4b84fd4366f96914743b15002f09738f8b89e7ce5292e30547b" },
                { "ka", "aef97e7449a8c9a4d1c133981c92a8eab2af0213c711f8e32abe599943efb318d0b4d89a7d75ac96450399a08f3a4823edb0794f4efc8af70d6248d56b8cf148" },
                { "kab", "4c6dc80fb3bb692363ad2505030a3eef448f3c4d3f4225cb4ba648dda03bcacf8abce5f2fd75db63af54a73df4b1ea19c5fee2c2f4e818c960bdbfd9f7db7113" },
                { "kk", "c5ac207dff3cf781b915ec48c4a2dcdc8d8c5e28dd392e7dc464f9b33a122f78499d84eed14b34dfaf070b1265ef524557a4cfaa7eed95bcff56f490112e1f4c" },
                { "km", "f4aee16522de451fd3c51abaf14a946ff06ff06d73aa56de690619e1e2ccbb941397e7ad6f1155134da23dc62b1308d3e7cfb96441aecedc3b085aad8fa79dde" },
                { "kn", "19da9187f0678511be8d117d6f0f8cfbb37bfc8d32e6308ab3a0be83df5e23ff21b442edb57a65b14360fbf327425abe9e4d632f19e4b9485471b5882b4ba231" },
                { "ko", "8ee6ab8e30e514457936d0ac8f038ac661755d438bd42a8fdfc31c2cdc3cb6bcda3c0d81c6747899d4cb32c86c6cdcc8b71024a9a0a48c443f2c0ab934b5dde6" },
                { "lij", "e84c19be65f3e7cac4e2bad53e975d4fcfb159fe3531ab4fabb6d4df08630a118309b468c711add955d575bddcb7725d9b086d74ecc7cdd065f9452ad5f03a44" },
                { "lt", "91f99e0597f2510a9dedf8081a602892f056eaace68228574868577be983a3708d2917a2cd8f980b5a208dd410324128f12aab4112ba9d2eb0a8d9096957af1e" },
                { "lv", "5bcbaa661ec0aea46a84d5a74daa5c2ad5272aeecb6fb5ca493c127d0ed9340fe203c5e82c5da5fd3436bfe5ebf484c11551eaeddb4469961250a632346ee715" },
                { "mk", "ad38bdfb241f9aa5bc6761a569f03fbcf00de54a50cd320947103a0885d621463877dcfd7df57b13a9b0cf075ed750794e154b868c5e545604e93b34e0952514" },
                { "mr", "3f06133929e6d780b7810176c364679d6c15cc38842d3e2b522e6c8adb74062a4dc2205a953c3f6eb30363e532bb670957b1738852d5df10115f55c3c58c863e" },
                { "ms", "329cc5429fedccbf5f6a9227f0381452a83494ec69ab5cc91cafc2c70bc447d0f58708e6529003f2e58df7bf505cf2fae02d0f010db1d208532d43da0e29cae7" },
                { "my", "327b4faca1fdf774439e22bf820faebb16f2bb600b1affbfa4359fef8917159daa7162b6d3264015c887baf82b4480d1d125267b32d152b608ffd7c6c7393863" },
                { "nb-NO", "cb4ce279327a73fbd74d9edc906387b47306e85d08610edfea5679118bd84a4ae453e77e4a7716594b184dd03a304a11bd2f7e83f19fdf4140710610a159069a" },
                { "ne-NP", "c970bf0c1679a302213fa9ee0cd8bc5509395fd9a1addbe30d03c97389a4b10872158554dfc0111771b6a082e60dcf095c7345ad7fd271e4d6c9f4c6c52b2382" },
                { "nl", "5ed4042460913874a1105e05fe91563fe186d0503dd54cf2023ad87ebbc75d1e4cebcdb069bb10f040de45684bc3f6b4e36780a787cd6da9e4de5e7960988df7" },
                { "nn-NO", "c4cf1dadae8c1924ead2949d8c53f518c8a282541ba8753789caf1f83061106303671f03e9ab892f0ae12402c5faaba30064a666638a588b96d421adfacae986" },
                { "oc", "a1272bbd9a73add7e8a48f86c9732a8eb69743892da83ac7fb7f84b4ffc6d39dcd412ffe817e2f4eec588c2d926ca09028fa037f9a4b86f619a9021351f9e997" },
                { "pa-IN", "160d1040640860935f722dcaee129c8d2971967a43d51a9e5c665cfb871ae51d220f75b5fe0185a44e4f42afe2df5a9fefddf61ffcc30f0d888de38e8eabd5da" },
                { "pl", "ca496ced5f0479ffd0e7a5ac136c2734bb8233a60b167ead385d8d510a4faef6baf8dd0b657794ea77ce2945a50d7db45104b53cfb376554705fa3d17472917e" },
                { "pt-BR", "ebf19a9323c474e9b59b594cf389c0ed922ef6e7976fd2248e758975c78ef1269aa92a0668d4a729af22034aec8bcf7865dee542b034a29d38b1506ef258857d" },
                { "pt-PT", "eb58cba60cb2bd3841b9f72d8b1d7e1a6229b0c21e1daf2b70084679e97436aa0bdbe6389424ab46bc80a13a057e14def6ed5712b7f9967a018d9c0beb3145aa" },
                { "rm", "b9313bc08bfdf7716f8d84140c9bfaf6dada48f73850e8ec0bc4d7b84e14d35154a768b7334b9e30c7822b6e44f97072e57c1559c17df1d9f6073fc30eec1d26" },
                { "ro", "8d861eafbfc333118b5fe30b790aaa07f2dc20f7437bd427b54ad5da5169d53c1dd83e02f6e98884e2ca7972ddd1664c63d83a34b182755c44ce693a474d7861" },
                { "ru", "69de8dc0654b9e34a7917ead1dece1070db66800e6540b938dd6c236947041d2668b09c1e79b4307e261bdd047513d3c4fa6bea16cb4d74adb8fe4f13302eadb" },
                { "sat", "b2a0bedb1653ded9ce47913e1ffb80512e0763cf6485c50e6897257ac2e9f460bfd781187b2f262780aa9deaf9e60bb507a8df4b86927710413e2a9ab8bff70b" },
                { "sc", "96fbcf3c81257d3227640563f6f1af83c5ce7e145d22a139096936240046ae4166b3b5b880a2b38ef080a81c924e00fabc792bbaaea391fb9a917f357a97aed0" },
                { "sco", "893ae2253e4e44c9e5fda8a19c98e1dc5169ad46a5828a82822db5e71a5e63b1be60a378ae180ac6bb45f742b4f76f4bd8ac8581d4fa3343c2fedf4bad83e53f" },
                { "si", "299fea855c3bbc1f738c4e56e38763e6f3e7f3f16b2f38c488778e2def0081d3e7548ce5f6b7d71db7cc67a1bfc7d6184a0fcb2df5edfc8a3f9fbafc750b0022" },
                { "sk", "fb97c04ca812bad9680786dc3fe7ef7a8044fad245621b05e313908b9201db2fa6a0bea9df5940a70d3a4fc63afd462b7e4bd793ad6f1acbee46f08398d0e7f4" },
                { "sl", "f5ee5eaef6dbdf2e5946a8bd308b5a6ddce60b566585af6371d3fca9103c15b167e9b4449698ea647b5f95ff9b970caeedceb2599990d5d8f2803252767e2eb7" },
                { "son", "2edfdd8da1d560ca9d5e928f8e65f62bc89bcdd1487ad71523be2eca86f645758a779e5ea49d151c0ac33999b5ef0cb2526834fd920e3c108650e1d39c13db04" },
                { "sq", "db7c2ad7f0f130b65a436cb29d8b74e9d312c21bdb6fafa1f86191aaacd534854761e1f3944518e0917c7a026c71f44487d65a721dda724aca72c61eed770177" },
                { "sr", "d5618e1757cb7e567b54905015f25f5983836d267d1ca271f7ffda2feee35f64af9c9e483ad5b158e6f3c3ea8b84a5a855682a9a8684ab388fea908ed827f1ff" },
                { "sv-SE", "8235b980e8597b5f1db1b2053863d92af548ad411cdd25a4d2bbaef92b412a2d17acebb4ce5fe77e61a91eaee2aeaff74a5936acf9e36887c4d01c9d1705f339" },
                { "szl", "db31d902430948131ba236edd6eb9eca18312f9d9dca8e41ec40c8fc28d6d7b8b87d5ec8fdbda32b68c478e9a8c7b3ab66047a225cecf0d2aaf330973da6c408" },
                { "ta", "ebb14777ca5ec37c8c8fae770fc882fd4c3916f16c763245cbc8c63af95b4e34642db1a019eb1803b17528f2d7f1d3dec608c73a2bec970a7dd70888b336fa3b" },
                { "te", "b37f0f90d7ce22e36c318fca7efe39ef4bf969da22b0867e511ff18af41dde971d960682b1d5a8a87685209872c97fc6aa8df4f508614efd6d51cd9ca98eacaa" },
                { "tg", "f59e68d0329d942c4a6c32309699b1b2d91d2e012f62af843c70ebee40a3b8efdc6c29ebbd87bedfea5ce71493af1fb0c328b58526fc37bd83626140953eef1e" },
                { "th", "1e6cbdf9c6bb74b635521524e9514f6a028559b16bcb40d4721f448eab95bc25bfc69ef8a077d7e6e019e309c0f369af1438586e25ce8ff802a747c3ae776fcd" },
                { "tl", "556fcfdc6c82e1949563eaef64e65b68b3d0b522616cb2e1a04589ac96137f48290f13afb8a7cbf868cb97c9f5a86cae3be25e13eec0e017769120d0494bd549" },
                { "tr", "85b7e26f66699666637f36174bacb6e4afbae67c41b9206aee02db0fc61e7269d70d743bc25fb94ea9decdff4f889662686250f3d03c96514e269338282dcf85" },
                { "trs", "b71fbbfada8bfe031718b25fc402bdb2fd388d5a337f2c650f4a639e77bb933678c4653ae64b6707e8be59fc289f2a267683b733fdb2e283bd6d343fd56af82a" },
                { "uk", "f19e93c9aa5cf80f96e1b3b18be72f8d4495c109f200e94f494e1b01d419e23af9f155e63e6c691d73c18261311f4f951f63d47a1cfff58287b2789598c0da83" },
                { "ur", "ea139bec9110eb9c16f2a256a2a51967ea2c5b49e997f6a13a4422b034e2eadff9c36ac9a0fb67f695348eab825b64a8354876167cafb0a8cdb1d07bf51e03b5" },
                { "uz", "a3cdfae44353640be7d4a9ad317cafb9ca8ef5263bea736a7e07e21702d3588245483337c85b93aff29af8e2cb3113b6faaf5836de836e2c91ca8ee4c800246c" },
                { "vi", "7ce5876ce68a9c59015423f01b1a11894674c3ab4ffc773f39ca2b7c63a58d6bbf28e4159d78c458fbf04b9a912596a83bef40f3be0662bee8e04bcef4bed13c" },
                { "xh", "02c996567d9ae72c077886095fbc362fa40f89a451b5de63fa51adcfaef11b93e12eefef3787ec07678e4f501600b9c0f327b2b5f529ed943e0549ed6a2faeb7" },
                { "zh-CN", "45af421ed81cbec07de8a0a8da0f47acaedcb4b180109f142c7319dd8ce8d356f7b50b5b696846001deb97f77caf8cfd51ea32b5d3ac2cacef9545b53feeb91b" },
                { "zh-TW", "94a53de2e7bf5f97f8d63adb030f23faf6bf0cf62e85fd19ebd6062d73fd14311fe5b8e5417c0cedbcb8be8a48a1696d89a560e7cf62223c50aeab83a6512d08" }
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
