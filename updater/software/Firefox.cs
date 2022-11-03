﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c73c9ef56cb595a6f7fcaa4de922afd5dd2100548c43272f5b262b593cec270adbc2d8adb2d1f3293f3d039ec0854d621e40d2be6894c5dbc234d77a4b74d740" },
                { "af", "a048e133e4c0e4a1bd58f1790036404b82078b569ab8907cdd927c42a54a1df90f0260ac0ae10d5633b60fa31e84911044b3c9fc405261f3efe78a31a1df1156" },
                { "an", "0e1bce13be70f03e7c54d47b642002397920faee028eeec3da57015498789456d7162ad0050dea4322bd21ca7448c517685600c2cb5a8ac4167bcfe625e50ba8" },
                { "ar", "9fea8686e2390b4af280addce5ffb21860318ae7cb029b2cfc9edcd304fdd397a768183d6406794e030402e7d3f1b0ae6d53f4770c3f094209c94c1f7a93d293" },
                { "ast", "43840d6523ef7c6e97c91a9ac7705c5025709e1326e255e83be8fab89bea58db15c6917d4350d2fdb11c22720854ca929acf66c721ba0f3ebe27148a1a458ef2" },
                { "az", "4f2b7a6413aaff49f39ee1f35b2f9e34e41ad7cb8662c97714d4bdc382212ded4ac0de193d4dc7476f8dd1f420677d9ec0fdf8fc694618a0804c03152dcb634c" },
                { "be", "fe41c0e804a834e1d9c89193af7b553fce5ab0c1bde369af949988f7d57112001693a201145203600921bfdeab72584bf6a5bdcbc0e44a6a10cfef68205554f1" },
                { "bg", "11d0035b4015a7c30ed205d55801d4ca9a31a1d4776ddfef361148d40a424ae2477dcf8a18ee50bcc6dc84f46a5adff5fd37c27707ee916cb3c610791d2a4c72" },
                { "bn", "209c689d87ae464d099ad80854dd613f1c61f63b2d328b41f1dfe285312523c5227fe1d6a44725e82e2e950df2783f87cac5aba19562fce2be2af13e7e89a4c8" },
                { "br", "3f7be5d38da7764c72c28107e9634c99f16dd823e7f23d4b8b3f604e67dad03c13b1365bfa3c870f0338fdf773b75016b8b5e1fe9840f2e82e36a557104cc9d4" },
                { "bs", "ffc6e97f0217be9372763c3adef6992593960ef8445c5701a73c0c6be9486afbdd9cf5d5fc8aaa125dd8ce2d5ac40d5138d863fca3eebb2941c2f0c7e7ca5684" },
                { "ca", "4c03304015cdb9b0b1bd87a785692f9dae04e4b19425ab4a4000df021f93aeb7e61e2a8815fa678bdb4dcc609be8badd6c55c88746f4a88aa2f4ade0f00d664a" },
                { "cak", "7c717f800e727635e85bf40403d7320f419a126f8d2cb9c1d167be9aa3f751996e56861dfd9f4df6c97ad77c892949ee92786e95182c494011313a18a3705d54" },
                { "cs", "5225591083efed576c6178d95a8c47b4a5412dde131e8d7494f8f616d5ecef279ec7976731302afe8e0e67402e622da4e6b495f024b89d4206ca32a1738b86cf" },
                { "cy", "2dbaa9142c382200e214d2503759ab6d80559db014ff225a202cf1ebeec54895e68a6620ee6fd457930debcb96b0a7b8be9fa28eb50014397669bdc9258faf80" },
                { "da", "5477ebb9f5f6845b6801cae4673acf8ee0348f08736b6742dcaea0852883dc79b6437bfee746fe2df2bf2a66e48113f720e8a4948abace973b8f2073e855d96a" },
                { "de", "eeb9f108dc123dd2810d9cf55cf32a3e147b56d0bcb664b2357c8be86f51ba2589c61be861ec5191e1a1cc0166d552fd5a3b40ddc07b3ec6c9102866ed96ad33" },
                { "dsb", "1106fb316392f82c0fa977f47e5dceb75f13a7041c890641e367a289402a5171b755faaebd82d21556f03a5416a59c7af467a3fb38a3313438e015e306fa2810" },
                { "el", "879e38eb740c8b452b50c436256000c32f9913145a1e171369834db9b8335464d4b93cc2c816052cf4dd299bcf9f66c65209614fc2aff80f5cc0916254ff8d02" },
                { "en-CA", "ac686b5f7c4b5646915148b70b38ff192fdb54a045277e05fe40a96b079ee6dbbafa09ff611f1234add1339ad07fc06e6c42b82e24906b6438a829edbfbbcedc" },
                { "en-GB", "37ac819fa385d2a421882627dd1709c171da5b8a40f25c8a995242af023684c3b9453f6f83645daabd2e12abe7f90bec33c84e76d7d030c834ac4c8fe47feba7" },
                { "en-US", "3388f590394ddf4f4a66978ad87ca0878e4e9001ea4ffd57962a4de839786897f173de04428ed5b26702cc9e7cbd21dabebcbeb0da1558b5964c969e09c6cce5" },
                { "eo", "97b98ff66e16a2cdae8a58884f2211ac6c36d70f46a3856d50d2bcfd450599b0a6d0bafc04a876d72fe7043e866e21bcfb53b66c21e2b869198ef7f3bd2d4123" },
                { "es-AR", "f656b0411ce0c393388d87c4302637a7690584ec026c15a84531942d52b67f8a3511edc08febfd047007a89dfae3c9c7fb13c2f1520c5f6909fcbb59b528dad6" },
                { "es-CL", "1dfe7f48822463db9a83dcf43715925c053a35bdf4ff42a98b1210c091723d43cb92d2a45cd13a182ef8d59e9c6747968bc4aec4db1f9cc1d1de09a7428359f1" },
                { "es-ES", "dba35409d0f409bb49c162269def51ec785cff0e19f2900f2e7295febcd1568162f80acc02f90c14637299622084a0132f6a7914590b9e8ccae466de415a8a68" },
                { "es-MX", "19adfee961ab3b7b808c764ecccdb84f1eade88bbbedd189c1b54862c72ab037de0253e5d3a5e789f23867201a23372be2e8e6daf24a79f23750a0aaa8bad5ca" },
                { "et", "628513bb3833eede5f64c1ab699e2866acfe56530904d73d879e216df081485c0619f70fbad77758ddb83677ffd3d4dc2fa0d441fba78c6040c313757d73b156" },
                { "eu", "d4acd27fbebf1dae1c6d37fd72f57fa1dfa8e5fab0b7e04b578c5afca6855f42a4c2a4b92da2fa63fc97dbbfe177529f05872c30e4dc7e30d65f2cd0925ce3f5" },
                { "fa", "318b76b5cc1d5dd6b1df28c043efa4021db592ea978a23464444b03c0469ac4731ef3eb8e2d68ec5826ed23dcc0ca2c1bb93eaee7d4ae76dfcd13f7f4ab036e7" },
                { "ff", "8f230efdae7b2ad4f40ca0ab8043c64a1d7ce0359d1a8500f6ce29986e9a122e6efc0090a109c31c3e8e20a20b85d8a77c9009891e950b8e1ca8dde5c786c27b" },
                { "fi", "8a1e6cea60cb7cc8f42037cad99a99e39f13b78159e2e776d49b7d75449f82261bd72f4107b31b2571a0b2a6cba68134ffc7c05742ff622c0cacd66ef85167f6" },
                { "fr", "3749cd09cbb06769b996e7d986eba66426f7fbbe4861265430228a56415cdb73fb59417f1ac7aaeed898224aa31f7ec56679504d7efbf7d84e5e4012e0f4fc67" },
                { "fy-NL", "bf7f66b71600d3c20d7cae30e91ea7b1ae09dc36352a023fa4d69513fb2ffb694726c6fb19e58164607f92254c549a69985465cf5552abfbcd2b6e2c5eee1ac6" },
                { "ga-IE", "b7280db8177646b48daaa35a34fc2a351cbbf5804583c04bbc98e40afe769c5b60bc9b766972b2a838f5c87034f3f2c10e0df6a48bd1726e7c20772652c28b55" },
                { "gd", "a9ea731d15c4aa3a32a24b8b6ee13c03b03c9f68b3b754dd4c8f6486944a1ab81f2ee5738190534928fe2768ec430a40df99acec9cc6c7d97995488c671c9d1d" },
                { "gl", "d1c2df0ce0505ca109383f3da4be7643761bbd9ea5e8126bb5fbb5dd8ae3b03fd80092a15b497aeb80862cf9e2e02a447a2eeb86fe497164957f7fee16e85777" },
                { "gn", "2f3fd274f1bedb9e32e244c5215895997446c5114c69bf452943d5f5699496f4fa0aa007e80ad5add624a00d23d3dd1971e76b191af047036751b4a10e478746" },
                { "gu-IN", "e48998c607cb019d063c7e6dd3ca0539c5cd2691070aabaef0adf752b18acac7375220ee75e8c2663e6ba697b7a8cd65e5f5bd0206f8c5621ba2405ff603ffe4" },
                { "he", "010f12a6982b9f20d2dcb465e2da40f47758fe9e0418232817890541ef8f1a99e53d27ff348558a81a5d291caad604568254e0ceed8667f66d3d64fcff399bdc" },
                { "hi-IN", "4920a0f4dfd5cba5d5cd05fb8b38a3a99d1da6e83796bd835b50ce7d9e783148f2de90eda62968d9be4c753e18137a830fe247493e3bb3f914829c8088e15eea" },
                { "hr", "d28fc8ae872f3749df6ff185d266b750fecc5ad14651e9ea880f635d4b156e44a88ebb3fc388d3331968bec958ae0a3415b2217724096b8619cc7cbd4402c1bb" },
                { "hsb", "ea81932fbca7871f2a2ef1ef7ce18d326ddcca4391dc1e54ae9c4b0ce65d4d282302a1b8f3fdc144140d3257db66d9929a4e7a7f4594d7425291a0db950f8765" },
                { "hu", "fd11c8d31464baecb2de7a4c3344ebf78084a1e083443e1928bc463324e65ce9b53d19f0df30b35f7e7e34a809753a12348a34c264c56900de301d949f2318fd" },
                { "hy-AM", "4648a67d188b50c9efab02439a0ac1230c37876498b06cd0aa1763265dead9596ebf9e35eebbadd66fcd2e1ea8954c3f121b1744c6dddbeed3840f58a3e082d0" },
                { "ia", "3ebae32707dedd42d5343c567906e7e7b6f7db55d174ed228819391de61f094214b6b71fbb2c5a091908b85fa9b9ef0e19320850a32f87e8832ac80d352f8984" },
                { "id", "f5827dd34191b1830ed31b05e2032b1098a292e974ca6d6f3d6a5aeaf1d0800cca3f2ef6a5268821897258e3e43f2ef87ac6eb5665b48241427cc88ac901fee2" },
                { "is", "e17f8eac09140d0452785bfc546bf03968f693f4b683d6053d565752a9a082467e29f7dab8ef39f5f3e13d9b505c9da61a80e4e930b1e0c8831fbd45ce691aea" },
                { "it", "528206422835aaee61990bd292134ed9ec1687cce78bcfd3ffd4a200c8b1d8b6a00f907cb399919188b42a75de0bad1ef7cc0ff896b79a32a62e2f0489b116d8" },
                { "ja", "a255e439d81a767a1044d60b0f3b7876b310cd1ca5c85d27f731f4eadc80a2521c7a0d0cad16ccd956b93e4e2cb0b7d436645a81bd57bb02c74294aa8f0967a3" },
                { "ka", "e99234dffebfc3c9191412851cf78ed2666a99d4652c81810a099f570aef65b4b829d8ef68c40eb41c7dcb8cd5f23383e6b435e98b4942c979fb2cfd23ecb457" },
                { "kab", "697988cf82ed9d55808d268acb8b8bcb4a8ff87a5ffb841244738110fe3030c3ba4266c2ff7f7de5add17c0d302e4c55b5449aa6b6d26821fe0fab78abb9dca6" },
                { "kk", "7bf21df9af48e8eee1d90a5c29292065b8dcbf83a972875d5535a220d3b6b9c7c5c2e3cd7d85580d636b4c88bf1176a108b82c4a4a01dca2019dca37909c8ee9" },
                { "km", "de8e62ad30f23867fbe876ee06a00fcc79cfb4b3c0454a3df548977461820af348687dd1f31d8c945f53a98e2835a3cdfeda4d5d568a27c79881a8d9dfdb0644" },
                { "kn", "f07ddd158835cd86d6798c934bd17a8db957219646e8ac4ee00c9e1befa23b85eddb57ce38295248e2ed5205521fa4336ec75eb4ddd916320483204aa950fc06" },
                { "ko", "fa7dbbd5eb35bd93b432a1ce78cd2e6c4f685dac04643a2b9e4cdfe4ce998f614864e5a73ba99cf17c0a4909810de377b9f4419231fc0b7ff7c4c1ed05ac007c" },
                { "lij", "48384ee08765abd5e8378dae2a4840c70fe32b0a8a60754fa76390d0cf1db3bee8e15c733f41204f994601b0229c0261fbfab49060624a3a2472a25837dd56a8" },
                { "lt", "c9ced4cb70dc94c5e1cc934045d42eded787986f4f34abcdc8dc737dc391bd80fc68b6e048c4f26dedcdc8053c38aeae052623ff8dc04b224c8eb2e01153a2ec" },
                { "lv", "9aeff19b2361feea1908caf5087c6028009e99af8ee0ced9bd6025b9b388f2c4b75b0a5b733acee6024802d1b2623c8135ce256ec1910550cae76961d925f369" },
                { "mk", "8eda243ed7b69aa089703a5d975bd843d19c8bff0e5abfbb65f5bdcc4bebc082368d2e31a8768299b26b60d1f7f85746d5eb4bc912279170db27f3e14ce3ef3d" },
                { "mr", "7f4be945834a30a58d51ce379a30ec66f5e31f51cf27da5e59600e1d2a1ee9632c1f2b329f76639679945f6e38b37238a0e81d9e210a30dbc384b7b5a0b63752" },
                { "ms", "ca7c88764f311bd5e5b34445c47253b14adbef1eecabce6a6b052344b0acf633e1eee64a2297dc39905a90755517ba3b627b50a932281a82a997c9fc7a1a81ef" },
                { "my", "e8c96e048b95cf575fbd5ee51053e203ea5ab92d0f60064fbc156f3b78b084fdc0804ed699df96f9751df68c6446224834577fe59e665693ec3018fc9bcec051" },
                { "nb-NO", "d399e2161d0f47e1f0bda1580db0c8c406c3d163fdb2cdab83f4b44bb69c11a027c6b373d14c9356e7fd520ce5a2846261322528bfaedd34e1e6262b33c5b5c7" },
                { "ne-NP", "1cf79ed7052b75682ced7ffd0c635f4992c62359db9c7c2058111ceb96138bfff3e1b1ce26e09ab5d1a1d94fbc13676f5892fdc0d065d49ee1fab54731411e7d" },
                { "nl", "b68323c9e488a1b460a565f32d88ca14771eb5e78ed3f024c3c8753e52732d5674b455e509a83edeff44bc7c137663d6631a1f9e126a4f4a71a702211b5832d4" },
                { "nn-NO", "de6867214d100383921f458ce020234f63493671a78142c9717be4faabccc3ec96523f9fda68b51b107571f3ae73dc0b754e26a51c3d6cc05ab668f9e9c11666" },
                { "oc", "146d5ef1901a8442888be0ae4a3696b022b285afc5a4416c0b3b9c2e8532e2b0801a6941a295f6afd8b084e554c0d94a23b721aaa9d84672fe6a1edf259e9ecb" },
                { "pa-IN", "8830c7f1d064811209c66c4ddebd06bdbde49f1cba80506d6f509c6c99df0283b162a32c20c3648d0688606020c84af49b4239e4d00c79d9b89238c2bff6d6ed" },
                { "pl", "b1644996bd47499f9f26753b23d70427d252a165eec642c5ece232cef66ec12ab2c3a42f2a1b37c41bb79738df7124971a068c8dd4ad039a61c2b3063e4521e2" },
                { "pt-BR", "2dcc638f9758dccda5bca6df3163ae08ae25692e8b8790264384fcd66bd31321f6247c49f59d272e4434b82fd540cb3f1393df2305e08bf9849f8c32f474a3c4" },
                { "pt-PT", "21532aa57b60bf3d0ac93676657bbcc184be10ba650538055496391921fcfbeae35a9e18ab06c8f885d92ac0ff6c3f8576d6a56bece35873082e78d5403f287d" },
                { "rm", "7252200bb05abf4417a159266ed9c02ae89b7a0855fbc00e1eeda31bf154e15d110f9626b42f6661f3d6f45ee59c715e15dc797eff9fe43ead0e03fa9b533f07" },
                { "ro", "421809df0d989388ac48ad0e903b30a3e1ee1e0f640770ea2dafd7adb355597611f72fd37808f8e1e45044b4059c68ad890da10e4871aea7e63f34c4af8a8977" },
                { "ru", "82f6e543a7386b74ac6985e48e4fad7772c6b1cbd5c5926abb95616fb1364cad3a74833a96afb0989e03a56c45cb33e5dd56f0b7fe627cccd0d26d7c1ba0d13a" },
                { "sco", "ea4d29ee4613743d97aeb9197d477a3f4c37828ede48b964df92a0b01f7d026bb6962b3b214a6ef7f25506203765421b453ee9c0773c0e6403cbafb70b596812" },
                { "si", "9e849b9739769cfb7cfa00f08665124fd0404953b436ba32347fe81b5a6c4487579d929a56a581c0e996d2ca63b45e709db4a9971ade730e46ccbc032629543c" },
                { "sk", "acc924972da4231494e6ab0efd8799950b86be9e118cc790656db0892d095a47c19506d4604b37e4f83de8451599cc41101be7a68375d53b05129e7a2471a9a7" },
                { "sl", "ee68a9a757e44b2f88c4d22c7d98bd9271c66bf64728cfe79c7128952fc9a52bac605872db6552fc1f70fdc0c337e68d97098b21f0e77608d91921edc1fdf42e" },
                { "son", "a9b5e9e8f9c0c46c10dafac56a9b35c92b3b8a7af0d47413b60b288bddeedadc597e78fd1b7dfa6649a21b76fd37af3836aa6e06f599118435009473dd184d60" },
                { "sq", "66c8c0a78cd2254322bf9997ff1c948c1e515d4c9d769eb073c4dca4c0a3edf067b3e09fac63b48fa4c7593c21cbb616f4740801e27b0ce71adce3f1f5f3e8aa" },
                { "sr", "0a3874abfd66916663a9996812d3e0c59f9b4d9e3f199b9de9bcd0f6a5c6ed8df1cc090f7dc4a825b08d1f39c53cbe5ecc2af753baa1541dbee28ddeca7827a3" },
                { "sv-SE", "c15a9dd3b729f1f8605458d7263319c1fbf5e74642afe7f6aca0a1031652f9cd5370f56d7b23a26897790f9fbc55393f83afeee811a02adaced91fcca220fe4c" },
                { "szl", "9cffa2715d17ac3164072d29d76cece18a261f942280ed1996ed9b73e7a639419ebb5b32bd5a9162f98a4f61b4f509378198b1621d8626882a801e519d524f90" },
                { "ta", "3fad2c84bbf07f6f73bc5dd45000ced86c34439f33f79b00e76036a3d8b804c3c8914b484eda73ff605b0dc35f98ad29024d1995f8c2b826b81add06809793be" },
                { "te", "d8a75f5fe720655ba79eb000fc6d2994ad8415e01687ac9d4e07ecffc801dc005461f4ad3550892da9dbd8cb4e751fe2e458190c1d271867bf683a90d9c107f3" },
                { "th", "7e00154e0d3230cd9f7bcdf21d1abb22fe9bbaba7fe6eedfc482806c58eebda247a7f26009e03075ab79b1c28fa0af20312b0dbb3a7a0075738bdf75648e82d6" },
                { "tl", "26380607d1b698b5d217befa6c3eedc0ac9eb60f12ef76a4b8207112ac15c6acbdd5958b8f27423d91ffbf805b69c7683139a654bbd970b0e534597e43138ecb" },
                { "tr", "d1524142b7044806c52f4e0b39ea88870881153ea29f16a91f8fc30ed43b38394e94c22c48647ecbda2dfa261c4db1870c74fb2f994744265e2dd3a7c8a6b1a6" },
                { "trs", "3839c6c427b97842302c360d17d3a5da5229b903f04d943ab94c87eb9ad0b2518d9f53cff5e17f81fb96e9ec58991a420f055b34872aa73dd0465d5d3ed94167" },
                { "uk", "a16e5150819c72f5950c9c91ebd6009c5e49b6abe67e15e071334f0392eff1361f7a48382259def8ba5b5c0747758c8629e7d305f44f22b2eca09fff738bc072" },
                { "ur", "3b5201644bf86daac7882c888a9812144be20eeb8ee6cfcb979d68e5707de3bf86bd7608a1cae7568847713d11d65c006e4e08f0386fb56e43147416026a2f7d" },
                { "uz", "52b8cd36c32bb29809be51ee0c2095e02265a11781b4fd9ea9455faccec71f9b640214807b36cf08346d7aaa89c8c215b7bbf3a2972056ccec9cf549edb7d2c0" },
                { "vi", "8e2cd6aa14b141b27943b23a17fe837f65f88034ead0ca4917ff0f2ca59645ae7613369be1c27598dd2f150890ad82ef5e4642aa81eec820f70ef16d2217bbce" },
                { "xh", "2646eb9463c14969ecf8098fbd9e6344688da222c4ade8ba27613c23eecbb5799f4a4db54dffb30f5e481be23384553370636dd19adbe0e0911ab52e0e56f2f4" },
                { "zh-CN", "62c7604709b1cdf05d214b63efc4b023d555a69804c7bbecb220141f7826c9f00a2452384a5e77d5cb36c102bb159610b2974d9cec7377fb2ee61907cf14c7f1" },
                { "zh-TW", "2d769ebaeae4c9d72ed7e9a23d76bbf4ce673980baed71694f8f4467149ac0ec8781269659703fbebecbdec2d398598622d9ad0b1d98f13611d67d7096e62644" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/106.0.4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "63567994e0f9ded24b3acf0faa824e2f6f4f3028772f163b57c8e9c242d2e16613f5c94d398f79ade810d705a19ca9cd83f68e383b8a58bd0bc105ce6d393f44" },
                { "af", "c3797f5505e6ca9c7a0e20bbf698b6d08d8e6b9f9a643af64f7eff1fe98e31ca72dd5db3a4da818d194f472999caf71f643c403b258d346202bf489d612cbd4e" },
                { "an", "425266e4bed270ac64c48dc6309e4f4a119c71e26583912dbcae978b8fda6f805e841a9e3309292361559c7f098494b92d4243505b62814a7f71888b08c13aae" },
                { "ar", "4de1a38b0e4f3e697e5a5968014be228d1cb92b8216623806be22c9b0603ee9be822eeffcc58cd22ecd71cd1cc9ae71e995897d89ee4e6321f1b4bb8fbe1b073" },
                { "ast", "dc7d2574f795a2ba7913e1564afbf3b999cfce53d5ebe0c787b9a5756d9b84cac019df5706710ea97433aa594d1b768e408907717284433ddd1bb69bd4f8e9a8" },
                { "az", "3339abc5f487ede4710c85eecf9966e8698cdbcfa04eff9dd62ad05c48e15cc1ea04bcca43bd337066f825d47a964137e399061623a08ddd20ef620a4590e58d" },
                { "be", "cf58044d4f023976079595a128f7022d80c460ce2b36d7b32774ca23d0e1c7f9d00238a872de2a1a875167df23db9c0aed1f94313db98eddcb9c68c0c306cca7" },
                { "bg", "ad5ebe87aa9b5ebc298f5b1a6eb142d43cb24bcb3da09e1217dd298a2e09a57cb9a80aaacf11ca4dae9512a6e034eeb0caf7edf5a879f6c821f24760c3ca5548" },
                { "bn", "7ed9a01d3ef9cda779f040d89a57bf138d465f66f16d84936416662c2a5580d42fb40fd70d6895d73429239bd337966deb5bfaa5b9ed5bdc40bce89e9cdd8ccd" },
                { "br", "c7bf4001f63b27d4918e8f49f5167476aade60c4aaed2cb32251a0d6afc3ab278ad23391b367a451799bde856b2206e2f29f744c10e37c4959ab70c439c247b2" },
                { "bs", "10ed06bf8c1d2942def68d2951975d5d02440e76a16274dc19d3c75666e282a46a11fc68b5b55dc27483af15d2d0c1dee8de2afdd14497c4265797bb686daa2c" },
                { "ca", "5e71804694d7aad18fd5ba4928400688969fc6589eafb92ab4995077748c328994581fc7f151eb40de9ac0c42989a94419370da9a80919e134fb6aa7dd697777" },
                { "cak", "ec96acf7a0711410a8c8406850e082bb2fea03deb05d3d213f4ebd633652a26f5ccd87ec8a0d3fc1330732faea96a32be7672ab05e1c732c93f8e50fc4db60c2" },
                { "cs", "219da86687182efd2d2de08144b75f883652a9c42055440f0d346127fecd14fbac307d0ac914add3355e401a4bbf37bd91efdb917467a54a88f0c49556b36901" },
                { "cy", "c42942ea67e7ae3f5299215710a67f1741a2188af3f63c46fb9e8e2d4de7e2dad0285c95e80ffcb04f26b17d42be64d838316baf186bddd846d74073b32acd69" },
                { "da", "5107309240fbb1697beb1b439c582fadeba6c777237a4057226825bab81ca0f481ec33e7d4a674e386421c085117699297f412c7ac395f959e576ac96c21bd08" },
                { "de", "da80f2111a1e6366239f57abe98623542df846835cb0f9ac0df985564898e2b7fa53ec2c1a6cc87460442652c11fccddcbcd6c24d9f9d5712cb2d1b3d44a061e" },
                { "dsb", "640781dfd7ab9ecbd98129f6b9cb0293265a58210d1d504b3669c879c8d97c7a1fe93757ef1fe9ed6af23ba09a21160cf44da9fb5cdd9751e83ba162efa625ff" },
                { "el", "6bb998c43ec3d60b086f0097e969011f1e376034036c976427aa33ec3264b29c697bb0fde9d405ba1b995a87cd877eda900b771b918387ce2a1f33938273cd2e" },
                { "en-CA", "47bf27414a1bb03ba231e53e225c7b9d47c9bece1c273568641f8c193a3225f418fa8c86e02c892ec0ae59f9d914435e1d5c96bfb5a3313f8201e0d849c8d027" },
                { "en-GB", "d98d10887b186ddc40bcc8161f9c01623a6aefbb1e68cc3efd6537f8cbbff2500da8ce64acab5be2a810099f54ba8e4fdd83a103300cf3693ceb06c9fa4b8eb1" },
                { "en-US", "76d5fa71834962517970873107f98724711bbcb7f73eb413baea573b2cfc07b0e44a5759b61176c56ad5592e7821eceb89be1aac6390cca029a305ab18b1ef9d" },
                { "eo", "34e00f976ca77801d38fa81bcd53b36aa8c6aa23e964c57bdf4401b4587af748605570e0e2e3c1c8702f9895030ef8099e07253b411b23dc103f74755fe32bea" },
                { "es-AR", "bff0dcba5658e624d98d4c8a6e37865051c83b9515ce7533684d121e882f2eb7ec254af97922ef2099f42ace6b7defb92dcc01ee94df4287eaac1ed7ee70022d" },
                { "es-CL", "b8788158f7b3fddb078b5eec0ac2bbcac48197c9bb3a3d26ecf0620fcc381f12b7aa313be476a3d20377be1b828e25282e7dd7ac899547afdfe52ed339bfed86" },
                { "es-ES", "9651200df8b31414949ff9ddb1f952883f53df59ba6b7bd4541ef00529953fa838d4382842c6477df5dc4a3121d0e46e15b84b37aa80da1170b58085d2fe5671" },
                { "es-MX", "5fbf18c213d83b7b6edd9d3b3d36088bf2865aa5e1d1c77f393c3560c139e300acfb900ca7bd8a306eae7a3dc7b9c2c3692a3f82c45084a7d7965ab6781f1dff" },
                { "et", "32cf5286b978e5c50861c6b88149e5c623a5892add4b611256b77bf9503ff389a8d3c0a5aa7307e047ffd900ef53d6c534979d000aebc343979ad8e717ddd246" },
                { "eu", "bbaaf61c9c6e62f6919b0cd6c3cb8bb9d1f30031ec8ab1a3fcb89714b62280dcdd6f50c8535e4d4a198216eb036a1d796786f5f7fca57155d89ab77c67b7bb6a" },
                { "fa", "8671b00cc289b87562c4b776becb697ccce69e192f9bf3934627cabd28945960b518985091899ca0e231856c1137e8513314184ee3f9f36149eec6bf57e58877" },
                { "ff", "f70dd8b5c34bd4e660dd4b1ec99b795e505969d25d0c7c654d09fec7e7d3676d51bc1f20785aee25cf5a7309c0ea28042e8b689b08db5d056597f51f66123d05" },
                { "fi", "0811e4d61590802b101c4e9f2976e33b08ff96239baf805423c2d561d7426891093c98c19c09073c71c235a8978128a576b61372ae14b581ae6ccbdb6bc138eb" },
                { "fr", "64bdafecb87a618c59537ee4fdc06c1aa9f5056d6fe57f3e3999610ef57c87c2347e1e7e316d8f10edadd01bc73558d75a3512bcdfea771fc92ce16af33e660e" },
                { "fy-NL", "f89e1c84ba780f409632de70cfb608485e25b183fa3327eb962496e2d2bc6dc7d51fefbe7b06f071b2ee1b60c97fec13c41bd76c4cf9c753ec5e392bca098943" },
                { "ga-IE", "c8a3a4354effc03f433081a994c1aba1ed57c2997d2259516140af6d40ba6c2bf95606f00aaa63674947604a677b2d74b7d0431f1c59832ddf22c3bc1560c7af" },
                { "gd", "c365ccfc1062076da4b110f9f0ad89c00d04049b4087dc419934fbf98a49889332039ea4e0751bf19dc836888273bbb4d0b093982216eb3620bb4eb17f8a7ab4" },
                { "gl", "bc541f35af588afd29c983d83f4c9ce4b956fb985097bc738277d0b66fda7a8f73d0b3209202c33238af09a3c5af536016baef8b35abbc5808840e085a7deea1" },
                { "gn", "f602d2ceee30bf230d3e473794d497ae3b1ee7052805b465d11f50384546931c253b05457cc0468ee36a612417c65994e18042e879d67c9bf4ef2ff964fb628a" },
                { "gu-IN", "07d6683abd022f3aaf30af979ff7eea6323dc2854c51a0dd54396ec1a5cd856d92ec1cf26d7a1d948267b55988efc42268c22b2d12bb85941e275562ea5bca91" },
                { "he", "97697b8735bc6ebb3b9545d2aa1de79e250bc6a2812a5ae8dc6751d7e2e25b9415a17707bf2ebeca2616f71264848a72f862ca442c687dc2aba89621bdd00c76" },
                { "hi-IN", "201f48d1c3b1a8251958027590813486457bee8630a590905bf3257e3744e31cbd2f63cff17382f870bc2b67a80c794c47fbb37c3ff754243d52eb12a73ff979" },
                { "hr", "97968e595f3519fea1141c2e61d2d9f465011b94aa36cc04a37995e8fa6ee519289cfe874ec8feff1d79be23015f7c2e78bf2cc77c7bb6d92328de8ab8d922d3" },
                { "hsb", "9034a85b3dd0ada6fb576864f9d13fbcae4a08c5c9e7cef003c02a3bfbc42ae297f48889468b8f3160375bf54ef8507780903198b4a800ede7dce289de9243cd" },
                { "hu", "3d06adaf4985358a5efac28ed5ab6d127274403f4a788e3ab09f2c700339985613d0dc1159e83f42645c25fc79a1f895ad87243bf099d2894d2b4084710e9564" },
                { "hy-AM", "02dc00647a868e2825b70086cddde999b52e7bf86dc931802d97e0059799b5767b34dc74f239239d41dda7ee0e824ba9d8b710fbef5917eda66c24dade0ef0d3" },
                { "ia", "3250544536329364684ed757c9c02556f68ab58e305397006770b0f2126787cccf6ca08f5f10d6fea790bea986b6bbac5f1f3dd201603e7a1a6921415efa83d9" },
                { "id", "4703ac1fecb7fefb9c417b4b1dc4dde62f9b41eaddaa2c33d6bd216ba13a844177b87d91b71d641df99b6b407f07ba12ae8099725f6a44e77ffa713c7baf7825" },
                { "is", "2263203321892bc7729bc44e7e9a19db3479f499b992d054f1aad2f30773bcb601c77409b73b017e2ce9564fad164876bc2ab19c6e4f923b9f6e272d3852dc8a" },
                { "it", "8a96f1a974d3f4ff2403fffbcc01728d64fbc1b1a7c5f1dde8c2242531b167a7a51a5a89c395e5cd1361b951e0635acff8300397c648c5a4d98964e187d137a5" },
                { "ja", "c4609875b855b5a813d92297035a12f3b1d492b12f78daf1930c72590f9950fe2d7c4fbf78054c1db4d1954c379161099084a28e9f0dad89b59c22cf94c7e4c2" },
                { "ka", "829478a0cb78fc5fd99d20d89ce0af0561c69cf7bc378353357624bcd1f634563d3fd5574a00d6d0c145f31aed9b87bd061df770a6b6f14e85acc8637d6ec803" },
                { "kab", "2a3565f4886e8ad0ec0f645e87a189e254f5c58ab92efbcdd5cc24e91f730a90ac74383d59b439c77f4a6c49063f6ecd3e65a7d281ecc94b5d718112e03e42e4" },
                { "kk", "7a7f3b1667aeb7abcc8239f19b9ef453b50e927090c243b92828df84c6737ea01c60814f35d7ed61aa1654cc2d3575c09f2ad7bd9848cc0bc3ed7c8f9e9901e7" },
                { "km", "634a025c9575599057ebd6d61cbf032e9a5364b38e52f1142f0a6d116827ca134dc9e283403b73ea63b93a8e385e2b1e715dd0a3185c9ab098d4e0206fd3c5de" },
                { "kn", "f6dc8fc8c2e6cd40b31c02de045c3ee6882cecb70284a76bbf6fa183f3a689862b805250751c60e700677a9381d590434b05f517ed85124c6e934433276a16d4" },
                { "ko", "5538dd6efa921fc03e836b73127be39bad66156691a35d0eda21eb1510619961a7da1120415029cc7ab0e0d41659d7daef172074dcb79350f7c455f4cb7808c6" },
                { "lij", "f97fbf14211fa3f652d1625a0f5a29fe8a05befe04e7d76756a5b51c61f1eec77ec2b156adb9a06a520d9749d3dcfb80544b64f391b9b576214f757cfa15bde3" },
                { "lt", "5ff2b8cbb2ada53406de71afec2982adfe65cc5563a0ec4d3e60efe2d6285a8ae52b6788d5d12d8c8bc4315156037cff50ecbf6e91a9e166245940ded46bacf2" },
                { "lv", "3b577159638aef28be4fd92a1071881586f73493793c759e47a89d3953874810ae0d2483b4dbd0c8d28b56fedeabcf41b433681b1476d2aaf2fd276f98d35412" },
                { "mk", "bcfd9bfaafa3abedc7cda8690722e0d9267aeb9eac615d40875e86e88c412ad9614bbf1e64d6dc82c3a749e2b2ddcc19453582f30a99c28d3c30df64c92530ce" },
                { "mr", "2c838705b80dc7f644b42fea231c19038a5c4e20cea6a43fe1d86d87541190537d58b7e8fead73cee650e2b75682858272272932a6cdfa420580224911164796" },
                { "ms", "d857a02879ec1a8759fbcb8c7028d38bba33d15165b1754cf12f8bae5ea360aaba890f3926a910061c35014161bb896a2c29b7a91696100867b6a81817b55e92" },
                { "my", "00e4794b92011aac645d3d585e308ba013a521afd787ce659517edac25243300d8770f4910b8fbeb0263a1ad0aacce377b2be1c91cb30430db6c6cabb7b5b309" },
                { "nb-NO", "8bcd605a255012261bfbf03946f8cd2f858510af9f724e376bd9a32e847e8d4cfc95dd06810e15bcfa088ebf82c13280ee58bd590d66ec5b6a40534c6a26f2c2" },
                { "ne-NP", "f6c1166a350a3a87facf59c5e7793bac7a1c332af0fdc32f10fe0be970037121133b25934fc898ada94adbbada567be0c1bc5f193c252d95157ed34be8811c50" },
                { "nl", "0786003acf621df031471ce772774bd96ad3fc5d5752e6ab3afadc253034c14928740739babd7d2e12d4b290e8b6cd6c049eb6456a147eed3c853b22fe6b9c76" },
                { "nn-NO", "4713151e6a8a14ac2eb128d52b78e7d8cee6075e2ad530b4a3ef5b29e9ef4f4819d08a2d5e3a1b01be076ef41a08b3af7235f5a4ddf2f72a84b324962337823f" },
                { "oc", "9120e89eb9ed8e42f30f43b88cf678d467f028dc82acabd4f06946c6fbeaceb34b978e9ff0be0f44c9c34f6b2b53122808e7f4a3b381e98aa9f6ca5102d6f66e" },
                { "pa-IN", "d7ee108d9d45618bef279f84f1456263aa83e913ce9da41166933eed6e8fa6de3de041c008392bcf5e1c3d4de46472a840e514ebac2343f1ff7c1f192b8548a9" },
                { "pl", "b031abe9223ccc39cacfcc5aa88e5a99f6654f41aa82cdf15cf28d39cde92f70cee53772ff768d48d34215e9564ec1793b54d86e285845950b8bd8b9d8735e5b" },
                { "pt-BR", "54255d736a953b82dfb2fdd9edefdcf1fb5e34361e1b02821b6d6e6cda9d7047e15a55da1afc287d3b26b75eb90e55b9ee4c5d5ca3643c29c4e41af01ccda442" },
                { "pt-PT", "58dd17a4bc78b2837103eacec1f419244da48652bef2c05cee2b6b44d409d8280967aa4ba1624e822e1846c726cb7b3cef75daf4d870ee29ca03a41c2ddcad6c" },
                { "rm", "897823e1eb7d8207bd021034d48f563418882dff4e33100044bc188fdc68426152c9b8f59de24916d3b0158de0e7c2a22037b21bef201f8d06c7ee0a0666f032" },
                { "ro", "10f09110899ec09f7948a5870852caee60217069b705784012a4d151b118ad9d7db1721b084ef26b6a0876aa0fc62e1c9203457dbb5db8d106e5c5a687d814e3" },
                { "ru", "0c62e7f3d55ade34371f4c5ebc1085c2fafaac6b1744f62059eb3aff3d96e1cf45a07e629fc2f12ca63953e4634936a74a944f027d8733c03412dc71fab7609a" },
                { "sco", "255506f2b24ccb2733a27df9e20e84fe273714fbaf79e52b8d9e9cf77617356856f01351fe32b1dad95ac0132f26acb940c652926f155c5e957d69283bdc51c1" },
                { "si", "2d561b2257304b7a88a28afd5818dd3b77268bdd7822c18f7a6f33f30133b5f2501f386996a8397ecc87e3a7956e3ea222f9b941f483767cc651ab05611426ae" },
                { "sk", "6ca082769e8ba45b7a8a08cfb8924c084bdf356b91ca34214f54be173cb3a7779470fd571e0db8e9ee113bf4e8412a144688abda2e20646be7e42149f460de50" },
                { "sl", "a9efff129c03d4bac5e51df47d31d222c6cb4ebdbae389a75b03c21e3d09aa10522cf733ce263e5c55946979a4a6ced202246c3d5a5364d92354f7fb462b9f78" },
                { "son", "4725a6646058d3286569cdd56acaf57a29e1485e8137d668894ce31addae6f40fabfe72393a6756cb21d11618edb19f6b86f190cecaad2ffc8562b2da41a2d9d" },
                { "sq", "2582e57d1d46bec83f77e7f21aae8bc4e5495e0df362e2b040b0df9ae5bfecc69f80937c7b4f8639e14e6d4cba1fc38862fe003f6a2933dab3be1a2793e25311" },
                { "sr", "a52b08904183a2d47bb906928e04776a57fa10c9ed0449f726b4b3a281cb54e22a77342405943c8e06937f5b02d94d113e84bc538eb630ccba5fe8809efcf17d" },
                { "sv-SE", "7432ab898fab0aa50923f07ebb299f3558b72f02abc8be1d069701634186d205cfece7cbf95cd91a441e169635e4a5edc253db802033334e397f483a0be132a9" },
                { "szl", "f38280bedfb24725e862025c6a506891ad7d190d870b47216dc666566ab2b3f8675bb299fabd338e9fe83aded5c4520fd8bdbd92a9893f6787f635c95b2536e6" },
                { "ta", "db024f097a59132d35a4ea699bf926ca3ff77b7eb79691589c5940ab4db2b96387b1d9698c97b969b3d941d35366227102d86fbd5c603c1e1cb80c1e66afef61" },
                { "te", "355a6d5c470de8ac47c6ef60afa5b394543cff8bcdfbff22f29b8fdb5e2f35a1b29b23db3fb71b71bcdb1a5d56b579f8db0c2d9612486b7df559c7e2509099ff" },
                { "th", "1c2e95c6db3a058da306abfdc85696ffafb529552405ecb723318a9d663ff06fdc0d062c2edaae3a86eca63b60c5056621fe485b4f1bcbd50063d65320cd68c6" },
                { "tl", "346d237735e0e14323a83c982d1fd10034033add0d77d1840dccf90070cd8cb074efcc1af263c4aef1b37465723777c7af645b7d4cbade7d2f5f2b626c245dd2" },
                { "tr", "9561ca768e5a2ff5113e238f02898036b3fd7dc3b77cca4f0e23409a96f2be3385cd8c4e9a40d2365d655a7e92c9428b08f82bbbb0d8e751192003a3ffd91a74" },
                { "trs", "73231ca07633259787e01a96b41938f28582535f0af6fe0bc8c78a2d361ed642f88c0a3375557195f1b934e152a7c8263cf166f99b86f5651dd80f725087674a" },
                { "uk", "1c7db2d089f2c6a50ae6c54875feeb99ef0b4c305dbf51b31e8d91899a2de3f1b5130078d1564a78d97f8916086f55cb7dab92d35d9b694e397ca36fef60bd45" },
                { "ur", "fedf4be763beb52cafaf0434b935e0bc61d68585c13de13e270ab610020afa498bd02d2f1003fecc3cbda2e4f00554c42573d5e2a89a8a701f646453f1c4f6e5" },
                { "uz", "e00b01cf5b920090dd4f223d8ad9b726ec1c5c4894904f273a06a51be6fd174519f0fd9e736fae2b89ac0ff0fac41828ae6736b8727284b1d7e9c8c3ec86ca0a" },
                { "vi", "379fde9d41676c615ede9ecca14ada080e52b33dea80cff9172fc0583027e352cdab83ae2d45cbde7a229f0f97192a59b90b34fe93f191a23bb31d0636f031ba" },
                { "xh", "32b1f113d95219f20a8826c58cfad1df8a2b808c38275b42dde443a65e7487b1529e26f7e3eb352f51e8bcc10365f670046320be3dea0f77d5b85344adbacce3" },
                { "zh-CN", "fb08f353ebcccd89c30db3fa32eae4bcb25474011a3f860196f55e1daf4a6c3e18c2ff3a58e3a689d07f7cb1c571855de9fe9b86720d349139c4322684aa328f" },
                { "zh-TW", "1ccc9ca30919aafe6acdebcbacc0eab1dda73e438e64020ee7d44a7faa9ac7627b2d0f64cb51c13ec2d19a3a757cdf861442264bad903883453a37fbe865d3d9" }
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
            const string knownVersion = "106.0.4";
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
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
