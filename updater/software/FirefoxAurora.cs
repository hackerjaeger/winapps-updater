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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "126.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "9266c841f02afbc1f478fc3abb414d4527a3978cba0155e873825c61cdf488e64fbc82c37b753b20c7cce28cd84683d999042f0ef75f5d90f34c8804ff117464" },
                { "af", "86ef250237af14a64f47ee74b89598be62c55ef51d0d89028e3ae5fb82520ee964e5a0f749e1cb6d4e5179a5a909c6df53ed71a576930b04bd4d37f16385269d" },
                { "an", "80fb7083c489399de2b0b1b9aac9dba0ee38bffb0d803c1abf3be330878b50dd238a49b9e6c35a3383db4a0fb771a5358384f350b0fb22dfc9b7a401ce208c7a" },
                { "ar", "779db2806904fd8aeebe92a4356054898661394f7d93f2405199c0b8e8339813eb37eb582aeec187dde1a2ac002cbf20424825223cfe47ca4cef15892b39e40b" },
                { "ast", "8cecf25d17723b2e33800dfbf5d8da9800f67919e87d283ebc716c8ce94b9717b6f8c64dd2e03d63c8bb585214690e479e0767de182663d6ef401de3b6c0cb59" },
                { "az", "5ce451beb025e8c7442618134727e51652f57baa574feda83e9e074be497475f931c33b84c36f8eb15633ec41aaa1af81f11fbf66988aafb58517a147743ad81" },
                { "be", "28527e0027abe29cd65a2edf10e89071497f6d55bbf68d60686557be0c3bba185a83fb75b767365ed176e2a3b1be89878f2156021b2bb9e0ae28d19054551609" },
                { "bg", "dd7ba341e4ed3a050455fc47b23a3ef967c47e4a67aba9e5c26d608fb4f4a5ac9980fa254f8da98b6e4fc8f2eda80b96516e41953380b8c9ea20695ce73392ea" },
                { "bn", "9b0f476025a630241d6af543bf0fe2f5dc0383686aace32444181eac70e1db5cd485b5763e0ad62033f36c8870da2b9764535ddf553f8dfbff8acc75d9b3c585" },
                { "br", "1272e708dc4e59758476c4488e76e71ef675f1e5e66a06a033b1730ede4ad1b2dcea865812ac860c7b2ccacf43f58d5291789ce9aa0f50eaafeef460b694a11b" },
                { "bs", "3bd510526054215d128705461f30fb19adf6b1831bb3ac0797af93c58cb0bc17e6bae5c07f62bc970efa14c1120659f1b7459cf65411e47769359e2a854f45fb" },
                { "ca", "3a3513370e53dbd54c1db6cb77f9b9fb9adda274750edc2c31b4d7c974df0dac907f05956f26d65c75910feacbf003f9e7428a65b6412a49b7f8fdbed224c2f6" },
                { "cak", "83202977dd155dadba8acb03105b0f9a8ec3b0417429f443bf69b06ec5089bf023eb092055592df2dc43f6d1c33916742e315814351f8fcee5a5567a45aeb069" },
                { "cs", "38eb1a228240548fcce9b7fe1fcdc6a733e266c8a8fd2f0f91db988b915760959fec1902c841d12418a442dccc101e971f388e5c1655baa5a16330316627484e" },
                { "cy", "b836d549d8dd4124ac38efbdb213cf8033381179cff6163a9934c20c7619b5250f83ffcd7ddc0bc1670c80afb8ec5488aa00a7dafb18fde59e0798b08229c26b" },
                { "da", "aad0d91be5da714adff38b67259b9ddf8acf8cb54b31f0ca23a8395eae25556667ad658c969eff44ea80f26322abfc780e0e8f5915d06185da72e36a9f9963e0" },
                { "de", "08f47955246953c2d5e440158add006dff7f93aa4e21692f584b77fb833ac20c7862f744dacc27f9263350f64a859824b983e11d3d59aae30a09a1072903c86a" },
                { "dsb", "119215ec487011e6dc54be395cba6e9d2240fae64c1ca1f3bc69a1fccc54f19a9df113a4ee397c251fa1b8253961bdd43971d2ad47d22a5160fc74e37c25eace" },
                { "el", "df2b7c4472b28366d68c03e38183d5c928b1770c7a791b914a713caf14cfd6f5c03b5c4789f9003cdd37ebc4b5b899489a583ddb71720e3b73e6ce6a99aafe33" },
                { "en-CA", "d880dc7731c14c61cf039afacde2c2f305db4e42ba82e0815312eb89724a8048c89fdcb44c606daee9ff87a78b8e4243653bbb0428f30820540bbc1e45d40fd9" },
                { "en-GB", "e1ca72123d62fa9771c8d6ecfbb0210a1b7b866d542f73e6cbcefcf8db83158d4f2d452f2565f3f0a672a2cb54f11720be2f7386e0381a0868dd259db361cde2" },
                { "en-US", "58c8e41cfa26d29d90716122aa711f3823f34189998645369c4a836fea109e6ec247bbc7f5abdcff3c425eacd5d27c726c31d988c6a76e021942db68be9647c0" },
                { "eo", "925c2150d1fd513d39be00db625724ac98b98f3a59dd1973a1b6fab8688e734d2170ba913d4d56885a484c2cfdb655d4408a9f08052273ce1bd4b6b85f931b0d" },
                { "es-AR", "42da37857b8b436ec5fa3f93551bb798ecdae036f149060d027b011b7d5766a6dfc521a4c1245b6e4362907accdd6d10809ed6b4cceb6ca965bc31d22a7d8cf3" },
                { "es-CL", "4176bd59f6857becb5bbe63054e732c625c8243c87003d053e6baf6b68874427aeee61b734d641696118f3e476ab528217e24422662262cdb00223010448bf47" },
                { "es-ES", "76a9867698ae9619faa2f9e7d5bde9773b4877bb3a56ec688bcd63ce498494d0b42cb79b82a22b1048838eeb966a71cfa59903aa36d67a03b86a00f2f4c42e29" },
                { "es-MX", "36ec79c383f68bbdfcc443953c46d8023ecee80f532262da7dcaef50a26abce9214034b93145172a73ad2d4ee4c3b2885b6938449f12767d35e0b82bb22d1350" },
                { "et", "0dd7b35319f2bfda881f8cc0381faed5fa2d5eb05098acce84f5b6b0ea8f7d08023533e1d351b656ffdf920a252046df314e82a7ebec930401ffba998428dbb8" },
                { "eu", "e462537069da02b3405d6cc9819b7c77898d5881911abb59aaa8cbe43f43c3d578e0f37f6f1fab577650fae50e8a85284850c66988ab1ec593ee9b41a5d337cf" },
                { "fa", "3332b69895e708abd6423a74c96dc1cbfcf1c19f8988e1ec48a4d379aed39775e8c29aa77e498e8edcd81db04c3c0346872bba5e324c83b9da924ce1e39ed786" },
                { "ff", "8537c24f2c6172f335aa3ba6fc97198141b181b7a0da24fac701603d8afc59adf13bf8038684603c583ba2981aad146d97714752a9fe6e320bd9c6ea6bbb3eb8" },
                { "fi", "a8d3c04f4eb8d96f4b93d691850de771d5bc4a3bf4881b863b766079bc055103ffa058a9a1fd15761f672e607ae2fd51023e3a4b7a4f388b4edb3f44c09cc854" },
                { "fr", "e99f567b8b31a01186abb2cb890b5914b391803357e736af695ae24115712e9a7420dbc9f418d03ca4a0343589ff8bfa3c310a398d78963bff6029f4230d4262" },
                { "fur", "517b2522cdb614f4167392fdce0076dc7484c29dabbc804d1ec7a42d1c6fd95e0bdc19777d4ca55ccd4ed60ab7c1a47566b4e34d58654c39e2670195c405b1b6" },
                { "fy-NL", "c1219c669f20379b4c7fd943295e4359a02e5c7540a2e4bcc504078df6de14906ebd1584509a8d313f05297ed3102e5b32c57c21398c386a6917f08e3448536c" },
                { "ga-IE", "7a6859ccadb57e9c8fb9fcd87f6b7d4218817dcd044194801491ab95a1f2457766063bc715ccf80d8a05343a0b4cf93abc2d8e0f58d47875babcbaafedc76e07" },
                { "gd", "084195fcab2c511b67031667cab28b5c7b6e798807810fd27368ebd7e5d6cb3e0ecf327598ea2ee67958b290327d6f813484e6a8f74b020723392d909b77f371" },
                { "gl", "d3efa78085d809691d3112554eaca9b89a63ead3664e7129a44454269a6d5eb5b6db27a218daa04e1dc333ce615010be736790c9ad010eaebc395f54431e52df" },
                { "gn", "9d9b7c386dbaa50e0e5bdcea79532cd7f931b676a1dadaa2e3cf75da2a1433cf21b80e29cd73058e3f74abf89f5b1a0d3c27bd5d7399acd0043e3cf39c018efa" },
                { "gu-IN", "06db9a6a648ddb517efe211819b0916bb83e31fe71d0520e6f4506232f4713d805af2db6667a1fe5971d9968d9857aef2e4208d44b7ac27190fcc87e188b73d6" },
                { "he", "8154ce6a9a22399056ee253857754d92b5daa5bf08760668d6ab27d3aad25861d7a68ee45781cc00b3af48cc17216d40dda65336ac033695e795d958093e079b" },
                { "hi-IN", "019353ef6dba01e7b4f44e91125a22fda06751271222ec4eb0e86132a71fa7c966e23cc9af4f595b85e5482d16e8e5d75096701e0c8ecc3a9b8337683f0fbdc2" },
                { "hr", "2d163f0ae96c83c5ab7933269ba39c33dde853c304166dbc751962e56ca4a2c0910de73ad15021f162f7b2037149f123e069c946500977a5c482ce98aa7be20c" },
                { "hsb", "56f24833308eecb05c1e89680341a7e05007c9f90a6413e5852f765aebbaa9170705598ec72b99eeec9cb44db1d0139f4e9e07496fda55bb0ef7c07b4679be41" },
                { "hu", "846aedfa320273722e721b306e8d034d49029fb02e71148413a258d18d84e91c80d3a62fe171c7c73c67bc8036588da481aabcc8e5b46f8dfeb1265670ff1432" },
                { "hy-AM", "c5acc87fdd235e5788eb937de7e7ebcb9538138cf038f29bcc22cb8ac3d6c26385548895e944ad0cf55eb3fe6e94b759e72aacb23df12b01cfc5d4f0bf9003ec" },
                { "ia", "f432ba3f2a65a9977abdcd67b3ca80336f16d5df27a068389bceb407d7416e17ee622f9643acbf3602168cc518047320372373bf9bde842ac6c8ac5f56727df2" },
                { "id", "e8c43d02e6a4a93524584673778228984ea6701f42ddd7bac022c104df6a9043ed178a503bec959dcab67d27adbcea9d653ac9a448c437c8672e72ce1ad49271" },
                { "is", "aee06269e575c14d0810b679c85d7fb09ab78a3a467a4c33f8e26b013e030029311a19cc7bcdc32838772c3e26c5d81e1d827c80eac237163fb8268a48430ccb" },
                { "it", "f8f6186bb40737e191ac544c52e7452c87b5b469b2841dd7575eaf49610e110e55af6e6a7c94bbbbd80489d078dcc68ca0022b57a8fa40f2cb6ad87946e8522d" },
                { "ja", "db5cbd28c10ea4e43d7a46c5b5f229e8a1b23882b5c874deea8b7c699647147b9548630cc390d39caa62d80f325f41d33f0fd785d69ba02898b05e7ba4d5f3e9" },
                { "ka", "e5c5367daabc3864268b58e1302b78bfc2fa159bb58977c59abb58efd8121b36b579eeb11c02103b1680fc22f74cc7cc6207aa3a97b52d5f9edabf2e51c11ed0" },
                { "kab", "3783cefdaf83563b194d89c2d657bc0d3b117d17eca12ddc542ea645bfed48f93a434a064050fa4f6be296b780860ada82de5e868a35676bdc80b7fa99e87963" },
                { "kk", "f96d8bb385955a001aa00c88da05f05737d06b64ebd05a2f6467e77cd2313726a3c68bc561370fa93ac043da37d4168fbe289571078ddc2521111657037caacf" },
                { "km", "5e1cf109d9ce9ac288a3f0f2b70822ebcf7f32d56eac1529be80be4b814b470ecb3e4d04ffb642affbe52d8f3f44ffb4f425092e594e34b4e284ab277ba4b646" },
                { "kn", "c757ce4dd1eeb0ce5705e424bfa5c0787b9f04ff3ec1a15a24ac686595a73741b6c3306a3abc67020385fac200a0186eebca1b8a5f92c0893877e0a8c0e7e81e" },
                { "ko", "92359bee8bd1506584c40bf28a7f05cf8f460fe17f4ec0a9abb71ec0a00a6aaa641a4160c62c65d7c311aa75abf75e485ca40773ba44cc37ce04741587337448" },
                { "lij", "1257fbd7cb80a996562a2426e3f572bff0a2120d6ea9a68a1904ef035f4401bc88c8f68ea9e04c1a95229e6eccd3f790f7d1a386ddffd2acc102fcccb9f94b4f" },
                { "lt", "c7fbd92e2024bc32830f6cd7f07fe7ece1393f1d8ad2bc0325a8949e6b5032c0605d067b6067b81f257cfc21d44a268b2da681c6b6ea2678ad4f490103add8de" },
                { "lv", "6c985282e383a5caa9ae9a959401585adb3ea5066413d1aab3ad5d885fd1a5ec504d8478669e0308d7fbf76cb22a58d76207052c99daec73110160e37aa83e80" },
                { "mk", "c3963699507ec56fcdad9fc784f39670516c2e55a871fdf4d069c54108811c174388e2989da4b2490a10fc52a6e8699ea0b1e56e068997ae730ab53fa446c710" },
                { "mr", "abf2dd8741aeeb03d7dd483da93ca1b13a77d7efc0aa0656e7c52d83b6615d9f7604bb1816951c5d2d72d022461651e9b668c265ad2120bfcbb67fc2d30a34a5" },
                { "ms", "555629145d64d622fd874c3e4ea818e969e260fffb791e2e16c06936e913bbb45523e5e5425966838d6da6c547f9ea477e84003fac71bdb22ad85020cfb1dc31" },
                { "my", "6034203acf0f46ca8b342cbc477e1d986a3efa271d6be854948e112bb7f813a1190ff386c0e13cd08e70aff3cdf3dbc57bee92ead6e074a238e307d6b05259f4" },
                { "nb-NO", "636d56c396287822ba2b9ad566b4fabb0d97aaf8637010d185723ed98ed3d825d185dd747098fa6ffc0c8888ef16041626a84f61454a1aac921953d3e18a0f90" },
                { "ne-NP", "3f368918eeba5c8de5088ce8f211e5d835c78aa46e7064d20a298efb3907e067ba5567fb3df40c6dc515be821b96324704a126500a804ab050f98d3b85086447" },
                { "nl", "755461e4fbb7990e5f453b9aa25b8ed1e3c8e5dd9c1f212b31a1269b96dcd7bf5ca765fdbc79bc3ac98d551334957e192686cfb593b380ab04f7b8b101edd9ee" },
                { "nn-NO", "ccc0bf45360583afa5ee67936332be8ab93e605d7a4f7bd47f5cb766b74b962d5e91eebcb88e90c52d9d9c93c6c68e8d5531d8d97bd64b0954ed3ce72871fe66" },
                { "oc", "311ea0e2e335a745b5ae12f26f43c5a4f5bb18b3d8e6417c6be77758cfe445cb45790fa73a3df7018c0bee1b5b04c7a88fb1a10e0cfe726ed18db24e05147712" },
                { "pa-IN", "cc7904cfebeb4c79afe5b9285b4b839afdc32f1caa3eb444330eb98744fbe0f110ab305df99dc87be033debe3f378a8b15ed639c6f84a395b0757d8b3d4aaac4" },
                { "pl", "f467d51ddf23719631645287366cf454d086c4126bf1757e1836c5f26c05301405b975f7ef811709d24f84b1c5e65986fb386d60b0055129965b27b827cfb5dd" },
                { "pt-BR", "a94fd91f8ccb4d2917664529f9077d459861f8a52964e8b31381aff9c232006a84d172ff6885e55157b49cbd483415e4302865e890cef7c82b4850faf9280582" },
                { "pt-PT", "f42c127f463886e227cbfa007acc29e86323f350e82a153d657278d79f804ea4cebeba131b734f952fde5ba0e0561f895604a808d4354a8e5013f99ec93e7f6a" },
                { "rm", "55248521f7ce4fc0668c46fcc032c8f951d0690ac90d43039e401040be938dc1c652ca8f600feccd0af1b4e2226da087b340875ed70c3702354b5337d7c67ceb" },
                { "ro", "4fde51fb5d984dbb47007b53f03237a86417cb8f257208be5071ee60d7c20e16665d2002757d5bf9c86dbeaf1177b34da629400f4907bd2b313c87f5e37ab22b" },
                { "ru", "8cadca95ee2afac868423b502119b61f2923720499df5c8ff343dcd3bbc0e77706f4a7791739054a964a8b3550ea20f4b05d12af5bc1f4015231880ab1014863" },
                { "sat", "c453ba7dd667344770d5de2f98e60fbcfad4e7d3057140497c8166228ce7b761457cda4355ffa8ae47cf161db43de64c8e496d38b144abe68b747119432232f1" },
                { "sc", "10dc867155a2669719b4b3356ccee320761b17a9c2615c4c54eb60287ec114170cea9d33a6b8811009dd93b7665a63548707504e2a56ce2f89e77c1946746351" },
                { "sco", "e76361455948660ea26c46da084d82da1a3db3783a9b18ac9db5bf50b5ecb8525bc500b083cc64a2a5e5bfe9fc47e41a5749dcded902e08bfdaa74c2ffe0370f" },
                { "si", "3beaacb97fc42401d2033b013dd15a9e75002bb8f033857d253bd03811ac5583753368f9400dd4a69140e632ca4b10977536b98f3e9a19b5885e3bedb5ba2603" },
                { "sk", "2639dadf6b0c459c252a99618a62c247e63a626ea27fc554a7f8e093ffe5cbc022bf61d93e301272fae65044881e50ab97b7ae655d23faa04c903d73253b1864" },
                { "sl", "589c52c1cca3111e93a4333e84597acf79bc1832d7a52edd04ea9f486b7e7ecc16f0d91e193d62bdecce841a5abf098cc64f0d01bf867c34977d318d5578638c" },
                { "son", "c8233217dcd1efdd774b11097ccacd791a15eaa4e706a96c114e99476daf757984fce8c5708714a9f29c531e2b13422f0c033efd7470372e391b430385ff6096" },
                { "sq", "8d5f24ff2a94736678bdaa094e386cdb43ae0ca60ec131ae1d7feea795dadbbd4a607cd9417de885ae37e69e09ed6fd7b7e14a734a18e20a9d71d38c0d86323f" },
                { "sr", "1992632c9c0d16898527c19d53530197c8ee373e7f55cff0af0be59395c5909e650b2bb00d003ebdab2013ddd8397b33d4e47aca4637d82bbccd185301463019" },
                { "sv-SE", "fd6bb832f7b1f7a9d9b0960b9fa2d4103d481839205a446b15df7f07c9b18090d77dbe2018176914db2eccba6a656475a16b45b128bdf001948574aad80fd62b" },
                { "szl", "ca958a7a67d2abd2dd412298e753d02ec8d7cbc650ae32df00dc2cf4f56a28882fcf33740be7ebaf6931b315f9417b8c88a1402cf8d3cd326af4cff75db75009" },
                { "ta", "33052c6ff1bd8d3d557f168c277f58120dc384b29b54c35911f292397f88786fb20428496c30940c06f99fa60a9f3169cb3baba17db8fc34c6b3653c5d4ec434" },
                { "te", "b71e0daf78eeeffbff6e75f7fd0e7e6a3bbe7b4528bcef228b99326d7d691311ee1d3c2980ca8bab62f124246c902ec67f23c3497cb79b2867d78e193702ec15" },
                { "tg", "12dd9e6f2d30cda4cf4b625a4e4be5ac5b1446a300e1f34119598d7b9587019e250195dd38b5d909046e24607ac11c4bbaacc74ffcc3879f7a24a7bd5f765847" },
                { "th", "cd51c06eee13a06ef8efe8481b280ccc8d4a2ae11b4ba06659da6fe6b4af12de8271650d6bd5b024e1814f539255e35a7b3bd766f004a2277ca6a395beb491ee" },
                { "tl", "cf2338c9c990069e860be60f5330170042f9165213c44cb153d0b7fc0f873df96705cba658c878ca7955af560a38f390da53c7071121afbdbc583e9b4b135f92" },
                { "tr", "16f75057720b6544d876f9d324d5ae2b45c25355f39e1dc976e269bf1e17fbca095279bab6810086b6314d4ffe935dd475da5ad3cab33daf5cbc953068b130a3" },
                { "trs", "ef2486818ca09998d2152e385315a3a972ca582eed229ad9be08556d3cc9d07f9d9e26caed56b39760c8beae71e25c95818a6e339348a6043e7be507583362f6" },
                { "uk", "d84d20025b199ccc43de8a7bbd2bbcb234a91b59763cd70b4ea5c5008de479866387f1388df55b960112d820afd390f2fe9840808d892346ba76a668553f4f5c" },
                { "ur", "3e76eef201d57f13de96e489c2be686b4e74d9d403e00adcb061bd1fa0ba7531389e261b57027b26bfe4c5b3681bdfda9db72b21a0a4f2b5dc13a5747dc51dc4" },
                { "uz", "fc02ca55237f2f49f2bc8dc0a822f7ac4b6eca52d7965a462ddbb723d8c1a58b2c25350202876c78b6c6a409e2e597381310eac5350a6aa6b437404d01f9eff7" },
                { "vi", "665c3de9893b9dabd9717d4589acf13d404919cb42f5cebe3236a0a26c75ee3583fdd408a0c1f26a367eef55cf86ecd81c647e5819f49bcb120abebc2be2045b" },
                { "xh", "93caedb69b9393b34f9b4e1362390b5983586ed2bb3d21d3b04b6dc027804890cc63b381a808781fa072e7fc94604f9f66f4a2ea5a2efd89fe206d356c5e0ae1" },
                { "zh-CN", "3ab4335aa2044b677007d193339dbc1c4782c4eb3c0ffe3254bc1a76e5f3c27bbb8935788d559f64e66c0c952e793c31e3f50436e41e83285e37964096a94987" },
                { "zh-TW", "cf21bd33edb3ccdd57c5531f8e8efa3dcf8fc831fc884b7c612442291a40c33791d10fd9dc5b79f05fc9e8b96d74bf4e7eddbcffd0ee34a54dcb6a5638a21322" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fba515c6b4b5c42d7731e94e34a19585a7c3d6682da756d7370d51a1e5687d4005d5fa65e02793aa47aa0c996cee677281304596c448addd9799d8bd3e3c67f1" },
                { "af", "5c54103bccac8eabd5a1c18950e20db1912aeea586aadd1423fad3c26f9db25be3d4c3ba5ea94f226bfdba7fbc1575b95b266a93a1e216aa02d75630b39c5d24" },
                { "an", "8cf0d8295e515f41413d3d3c007b8f1cb9e7ddbd8a7b06ba5896826273f68810e783975ccfb0ef96ed9f3d8cb21eaba19a340b0ea702a1d9acec0a75de006cbe" },
                { "ar", "204fc6fdf6da6d4906c7a88f76a37e869dfa3556a8a79018fe8f5882ae4353b9df575b092de4940a232bb22c822cbb289207ee1c3431edc17f4f3f5cdabd0bf2" },
                { "ast", "66ae4850504b3661fe35cba5a9b443a4b20f5a7b6438a71937b7d8b9700494a9afd891ccf6e55db5a1fe65060bf00a2af842b5769b56ea5478c9e550f033343f" },
                { "az", "5603c2a27020301b782ad763f9fe471d3f42cfda4015ed35a4e369089fac600d5f943213930571ca856818aeaee5b4f13fb7cc63e59751c49eb369164f53ba8b" },
                { "be", "1c4e3b86ed6052f1f196bc235f102e8c478f405607d5de22cc41a1037b31b345e346310f7f467046190dff3cd44e75af6696126fb62dd558d2d2c2ea02c16f7f" },
                { "bg", "5a7fbd577b1749106d558ff200906276e0483b605652ce3120b2a2f157574e5fb46b7e6132c5ce81d4fa5ab4828bd415894f7a14d6086f6a84110b558aa42b90" },
                { "bn", "22ff5f553d1844ada76f92b08e0c6285f3e9ed45b25ec9d7f300312fa0329178fb6838bb4864f0d78e89134991984a154e9448d2cf0e4dfa0df07c7c0145695a" },
                { "br", "3080567c3cd0928ee0a0888b362f776661582a75c9092a86203da86156f15f4a3f23ec1081009bd8cc182d1b72d70ec304a53d7dfa7b69185b13cf67d3958315" },
                { "bs", "6c4f15dfc161350a36e2e4212de3bc2826385a1202c52bfc88fc606562f6fa68e9de8cf9b0958f822cf378dfdbeb777fa65dc791d4c6a3f252f11acc1de2f975" },
                { "ca", "120da86c92f016225ae96e5d08506a7c72c221462b6e7631a0cc805d2c328ef9f140676013aca31ba65514313c269f1e9b90e9752d38d50d5af66b311906344f" },
                { "cak", "9ea491f7a9e7b36f3f102175a56ca09658b022b18334813a4741b60f09df7053748328b7d97e42c9a98caa9cdb1ae0e22356b70339ea8650e81887cfed9bd330" },
                { "cs", "e8f06c9a58e3cebe3c32ae2a3a362ff1be8692fb52d36e32a756a7b023c094ff25c63d6ca0fccce7f014db825a0a1f60cdf47b090aa0230d78049ad5ee303b50" },
                { "cy", "2305cd2466472e393d6bafcd95dbffab8b863c11e28e38c80bb5245dfee308076e5a334e20639c9ca8782a955f430561312a5343b8324df8ede7f44fc98d803e" },
                { "da", "7e0ba0b287cbf329f2e1f9a8944d124a3165cc11fe0224f0fcf994969bf92d338a90459b16ee4581d77f04a439bf9fdcf5aa6209a33c8e564546ca7b60f2376d" },
                { "de", "0521570f663d8a6fc8437e04675b8fcc6c66a3e5b85dde0fa42f9b3f180f90052b6809fdeb8ef94622965a91780116251923d46420304e5a001db9f8da013742" },
                { "dsb", "3f974c57ad4872812f48db82815e5d72cd9fe17a1904e9d17c8a1db1578d3988d77d1247f096fbda13a7e97148eb633982c60130205078007c2cc3bd283e5ace" },
                { "el", "790723981de0b8fdf650982d283e47ed187d8e5bfd60d1e5b5109a46c6ebff0be4c1157dba23e8df3ade902af60224d63ff192884239a9894eaafb73757acf22" },
                { "en-CA", "e6c1f743de248c04230a9d06365a635fcb0fdf31b5fd154ce456b7a9575648ebe2b803e464c2788bf04bdde7d8dce61dd797938adcaa8b0b3c9691e8380dfa5c" },
                { "en-GB", "1fac9c0ceb2e18b7787940f94b00aad6142a607dfec97aef2b938f0a147303c5554f1efce5f2babc59f619b15c627de510b2d6c72a9c8f8714c4fee65f8023f7" },
                { "en-US", "ae9bfcc55bec743c2fe7fa2ea9824dfce131c9ab13ace22d3c50fea7de8c75613574b101fde22a0adb413d944ff7a56fe712641dd71364ef79ff1d34874efd19" },
                { "eo", "b85142af7c38c08adb681c11a24f25a27114dc8d8678df5dd62b2d5237c6464e46db972ed9274eda8b8cf0022bbfdae1b335dd5bcb7fb9869aa0ccf2cd671d2a" },
                { "es-AR", "e664e6b49f42486094b137fcfe11e22b1dbceab3cf8628ed89c7c0daf26ecbd50547db853dabec60d46b09e111e48ef975b5e16528b2be725ed6441b73756a79" },
                { "es-CL", "143da9e5872edd24dd6dad0d3ee3385bd52dd6cb0870ef5ee82caaa7827ab12653b0f3c1f483e5ca2431402c4b7b095877508cb165693849799c1ba6f2d3fb72" },
                { "es-ES", "7feb6c33f818d3e36ad2890da6dbf40d315532b386b4e908e244a0909dd96348e9d54ceb49e9ed6b816c434d46e944b0ca342db15f6ce10f08d358a8ce9de760" },
                { "es-MX", "a6bd31f49efc39d4b5e8640666bcfdbefba8dd71d8eabc71725b846fe2cac894eb8e2a97758eda61baf9934f5343cad564029bdd3bdf6777dc9218013bf229d5" },
                { "et", "50b7b19e2b5bf889b2580d004098f80824c6201f9f0aefc450bc9c634aaddeca773632368482e08b9167427f4b9d4407d25b1125e10f04ebddca156404693d9b" },
                { "eu", "19717a36b265bb002fb0d957ed16b35970e6c191e69ba72ad01a93e24cd308d07d3a9db5ab87fcfa828dee9b12e1c8f6840e0d563a486cd1f811327e4290663d" },
                { "fa", "264156db98d616689768c3af9b831c7edb817b6db707595b98ec221cdad9e97703830715a4ce62d0893f09c64912d5ebb467d961634ea86651f54ff344412268" },
                { "ff", "a94b542a037d5a27b9aa94bf6ce5a98fe32a50e9e9ea6be3fa74196f740c3a8cf5ce9fc87cd3911a2e622c78be788eda935aec9565dbcae789ab788438144098" },
                { "fi", "7c3c34189cfb8e8b17832c3b4f94e15274b6dd993af0b45f8c37bd85e72023f3950997017787f6ca91b743527c5d9ac8d5350f42e99102ef639c386469aa00b4" },
                { "fr", "2f407727a6f4587b52fa7c4f0e0cc8ef25d35dbd0434752a887019a564741ccaee7090fa2baf7fa5b1e67808a285914e337d746b635682f5cec64c698337f244" },
                { "fur", "cefbfc904a9563ea1519627407070c8121c80107647424de2576b843d34f78c0485a109a716d2901df848ff70de7c325b7e4a8a58deb7536d9d6e328031ea844" },
                { "fy-NL", "8eb9ff43f5f47020aaf37670769b78cc7d406ea85251f072b9f749285b9e00379815b5fe8b3f01bd650d71d1a3fd24bf1755045d084dbd41a18b7772b7e742e5" },
                { "ga-IE", "4a664bc18d984b8041286903efc81c64fffbffe6eca4cdf103b244beb21dd37dbb34af864db04c8bb57393c39b1abcdf69d643b3d40cd374cead28d32ceb1bf0" },
                { "gd", "6f06ba53c27592f92c4827c151c9e55e3ce5a7c988b7f39748308f85ec8e34b35e8c389514c97f0a8f5b95c98341807f0b6aec9ce0733eb9620d45833304efe2" },
                { "gl", "300350e32d17b8dbc7ccd2b2bffe1ddd8aec95d088b8f2dafbb07bd4dacad14d17675a93bb24ffa781f17a828717eb7b6633ca4716600fc287caf497379d5a13" },
                { "gn", "027cfa184baf49494586408ce73ef223b27d21c1c08c2806312e871a2f3700b1b6246a53df86291eca86112c8fbf281a01d31d5bc23bf10bb53f3e4aa7af8487" },
                { "gu-IN", "e4b0a0d3b64927412e19fdfb095c4cf6c9c99cca70ac143b0a080b834fdedadece986a38e044da2adc376510d3ff83aa01982cc47bf345411e4d609acc924590" },
                { "he", "79e3a5118778a0699ebdd378e255314878d86255ff0665e92b793a7b3f3da5db4eb0a007119e469af03c726095db61478a77fd59a14863da65846085235c302f" },
                { "hi-IN", "553527095cdd0a5260dc438a2d2a847e79e1a91e038d21448976141e9ede797baed7faa51be511a9fccf6fa7d96669f1c77547d69de736dc516b5563c76f6fe1" },
                { "hr", "76abe42ad8d86b36f11e204f2a79fa15e5feb0042e58da5e0f9aad16b09a95d6dc6df8b7a00bb4ad0554974cf0ebe90b61da1a5d9b3b9fe9939ba1bfb8cefb5d" },
                { "hsb", "fd5504f0a6059aed4b3c73ded60aed70fcd9cdd26bf2b3dbec155ce2389135c1f3eac571ca9028c676228d7ba0bc6094307f2519acc2e84fd254fc39330a1a79" },
                { "hu", "2e73bd9846bb3540c69bc2ce8cf430512576f863a6952106ed5e1d5391d3feb24979262e8c1fba75aa957a8d5a95a79a0366b7d29b01da52c7de3a5fab86f85e" },
                { "hy-AM", "f7b9f02362d8511faeee5775349e614a5efbd096554d1ebf1be9d9a0558763b0c93cc38fd8d8a01aedacb4e2297344cea5abc4cd840c0b6007e34fd5828a4760" },
                { "ia", "709d5e3be183d41617561382d8c43cbdffe905662d457d572fb08923ecbff58bf3cf330f3741870b81f2d6367dec67a036a3442cf23e9f7d0d241e7329cd4d85" },
                { "id", "913c5b0f8416c7ea34ab3db95db5e7fd407c79b6118b2538756fdee7d208db3f75da79a192068cc8fd6089dcc24565f453bef3e164fb49fba316ce8abd3c33d4" },
                { "is", "d9da838c3f4ccf0e918f5bcbdb1b166b9b997ef4f8838561fc64229c85feb4c646743f4c8d48851685286a8e7fdc8c3a9e6ec217430b669995e74ea50061f4ab" },
                { "it", "c8ff6feb47de17b7005e2207cf287edc18f4a3f29d073f0212dd057009e57638081d4f77c5f4dbd3f34f0306a2bc60332c376a487d8ba0cbc63b85f959793b33" },
                { "ja", "69a34d05c06b405512ebfe21fd6b236c009c66dc6cc757361e0344bc0b22d62f911fa0173c0fd6b861362589ea5fdd98017631bd47e1d7823fd17b08970a7627" },
                { "ka", "81b5aa65c3e54e7657e3e25bdac5ed79218e8f89d3a7b6aa30430e4e7e9a196b7de3ba269f84ca0b1eff857dc89a9b282cde173c9f24ed5ee6b8235a2c51b222" },
                { "kab", "8341c455d41bbec57efaff27c3654c0e85b8ad12a455fe736bdad7434b787db72572aac86668c8a016066099f1bc557c0b18de0dc634156fb9b44c5467dd2cf7" },
                { "kk", "f7f68613f9cb707d1c5f86a618928bb35e97fd5e98cf1ccb241d518a49eea04cf082690a5c4bd6a939590ded1a54283433c910a0458cac351e6ed5d42ae4f190" },
                { "km", "1db6404707c9e733961036b5defb9b1028b1464cd30ca5d4f865863d5ac58a5b7cb7a90246c76c8f0bfa40145649c792b95e887756ecfb681be93797d87cee98" },
                { "kn", "cfc3f936239932dbfef2b33a23c8da6f9fa8634c68d9e077f1990c543ad9e2e56b3963c4753f1e256ad558450391f5ea809b3afb192d86683e6e15be84269a19" },
                { "ko", "94b093fec8ebc71cc582cb0e9368f3c42a681d98a6c8bc7e44cfa6fd9f721be0552cd401fa2ff2626a54a9d75b9bb77ae6238bba9f43b668f9a1eb1dc4648d2a" },
                { "lij", "2d2807f8b9ea377abedbc388f48c1a33d9cfb579078a6a602a632178ea07612cd70018a91dee206243f53f5d95b12a2faafc72fa6bd18a80e434d8bcca0f2e9c" },
                { "lt", "09dc59fd9eaf17d9cd6962585dc9be9bbeaadbe4b36ea73ecb78296e4d95ead82c71267377ec0cdaedbe72486ee2c872e967da1df137d3932ac54bd976334074" },
                { "lv", "fa2ba2f1bdf57766618d0662475a485ad5339c9c8ea203ce460b114bd42ef4bee70585e7cd2edea1d89e88fad83df71ecd3c46caaed86a9c50ac800ad29085d5" },
                { "mk", "48b6083f8226c8ea545911982914852fd2e2679e468d437e908b8edf60fe99fabdf8efbd79b91e52e17404c44143a1d6910483921844a6858e8ff8789bb422bb" },
                { "mr", "1a8f5b8b19f162cdd53dce5d8c867b4f5a926fe134f6aff23a870b85455eeb43f1fb75755022d6d1c301ed50394df50542a0d3aa68a6077ed73b26cdde4e2199" },
                { "ms", "6547cbd97982a08b2491e2643a0de6020409bf5cb5c56cbf994accb6ba3a88c4421983cadf7c866aff7669db89b97f2a78fed859f58af4fae91c6b1ceef872e4" },
                { "my", "14d332387109307fe25aef874f3fe696badf6fb012c94ba340ebe44fa11d2e964f89824212d8fd18b20278b6ec7752a9665b3848d95c8b26d0d0f956b809a7cc" },
                { "nb-NO", "fe992f197290220fa29a3a0a46de6b9f7c1c17c8ab23c446e2956d41dd34f6d2fe15d02a2246318f464b197b21c91fc84f61e6d00c13f143ecaa3528227d9c04" },
                { "ne-NP", "443ae105577ff8f54c0401bd398c46171e99ef4eb120f95f517fa8a7dc93bb6097a8c2e0661479162c48c36a02235849193c5fc722ed96d2d23acbbd9e765f2f" },
                { "nl", "efd1bc77b9829e5d23f6541aed16523d34f8bcdb94f03a3645cd0d52f47709b92f1f02a6e6697f0cf463e95b5ca938142023e362b1d1b82f4381b2985d297ed9" },
                { "nn-NO", "bc95bc7c0492f886655932f1919e95935d68038bf4d26c29774ed80e7fa7a3e112eaaf049ef8e7a83994c51ce83eb7156dda7e713986f21ed100254c2f1f372f" },
                { "oc", "165e029d9d67ad8114410ae4f35e535be6be5cb72e3095542af68754e94dd438e80b45a45a23c98c29af617a3649e877bb481359eef4d597a96276746f76f876" },
                { "pa-IN", "c1699dbab111ba1e0f5db191e4127aec0669b49384b5365426f78a1051c5b616215ae7bfc312f8e5276126f3619d0de50cc0b66fc31b8ae09f859b5e0bc7a3ec" },
                { "pl", "15500b597f460f73c47f5a72ab90f0b5b2a9d26ef1b6f3a2f31a4a5abc92cb111fde968751a709a9b6f6008f30171eff4379d05e996eb6422c00fb4b000bdfda" },
                { "pt-BR", "226e45f74eeeb7bdcd2cac8c6b7dfc911dc7a04f5cccaa2595db69b0fb3e13bad7fd28cc63ec504dba155f6c3882b21545d4c8664bc35fd82bbf312d50367860" },
                { "pt-PT", "687ca3577f569016846e85f507322a056c11859ea052cde4f071f5e3243a6b441e8d092ea061399fd3f6d0811b04557f55dec39a7007cebf9f30c08dd68f058c" },
                { "rm", "a2b958c48fbd8ec42afc1a96be76fedce76b6a30c1854b9fdf7c04f50f0c58c5c82b191129b2d2cee1df5e763211510d296a3a4c4d3bc9e522d57cf98e578de2" },
                { "ro", "41c04b3747a4694eef8bb0ce1e031a3e1b5ca977266959ab0f3b777e2fe91fe879ff3ce724e06f3c90a10da9f6410c16543ddc77ec62f6127a6112f4ab42e5a4" },
                { "ru", "6579d4f335bfec66549ee366f4c1df9d02c2a3f496d0b3ad646e262b231c4200e8639f6983472f5c2a65a45a70fdafbc64e5bdf3270248fc34d004465842dd93" },
                { "sat", "6e17bde29be9ed605abafc49bcfb9df095e436ce70f58f06f53a9705a951484fd38ec71d19162fb5ef25bf4ba704fe83c4fc88457f5149052ea3e9a7ecc71fb1" },
                { "sc", "8e604c514a05ed5abb3e20af42b6037f4590b16883072631a381f92a931d3f4607870e73daa18cdc897e1c201957db673b80323ff729979448ff060c1e2d2525" },
                { "sco", "17eb36ac63f190320bea3484c9d8a8b07a14b8af8b50a86d399d340d99b83f38074295de036c067799c6e3acda2304b80dcdbe6385cd8731995380b33f91f057" },
                { "si", "e017fe40b899456a7b67e7eec6b69e19724cf511b1af02e11863f5edd281ffca3508fa57b403721d15bf910b1cd74c265fc69c1a876b3ca794d602979f211c56" },
                { "sk", "6e33d9b796d32f9ad0239c98515de1f6fb8cf61cb2f424599c6131b5a93cbca793abcd91fe33d3c63df663d6218db96b7934d84c2dcb1ffddaa71dc6ab983a25" },
                { "sl", "3cbbbbadc6fe68c25dec4e5833c3f78ceb7a98f4cc7449ef1c0461e1fb55d65947b0d86cf62f0e86fe31971830cedc5904a1d6bc5e1d9b6b4a74ac6a48aa047c" },
                { "son", "912a2db6a84f286be1dd9c36ca004fcf5b9c35f8aecf5c26096b463707cbd186d345b89c38d40efb056ed67ba1476118a6de0cf2d1427e865e044a3e7681786d" },
                { "sq", "98f6e3c76c30a6218b2377b436ebb99452d31aeb85adad35efc1a0fe0725e9aad227df0c7ffd2c81880503ebd306bbcc12c96abd539d65d4c7a44f86a2eac55f" },
                { "sr", "5629d43b7dda698049f18b09cb6403901c9cf1eb57f7aae277eb9350fa2c84b820e4cbe732175999c7bf5f13c03d4e3e66dcb735814271733e174a0c349b6361" },
                { "sv-SE", "cf468cf0f2b38645b0162614e511e0d9cdcdbb27ed5cc7e591155b3a8dd8b33f657017a0a6844dd59ae76337793f0bcefcf994ce0457404d6bf0c89da4ff1f23" },
                { "szl", "ab025f0c236071bf6cea38e48d7381f9a6144bac6eb051a7b7947ea41936150d152edea380fc9c8e9b4aeb1fed19ec9c05f06a137c4d8cb5624c5250f8216a05" },
                { "ta", "f0b82e6eea29449afe3786d218f87d85bab7e2a01863294b86815c83290c4849eb11918699f630bb4b4a240e206e01d189dcf848edaeb23b67951991f3dc0770" },
                { "te", "4ee9f4a95df6afd50726081f68d28eb78714960d8d9a2f995723dc358491060a5fe174ab89235b39d0e379535d2f7e4189bc4fec794edeabe98cb6fbdf6ade35" },
                { "tg", "d2e883bb2dd124c295287233627441aa76d9b15d80722aa9c872774d85e8e2c506275540275c3f1e2df66a44d832ab59141149c6c0ed3283b5197801ea1f41f6" },
                { "th", "7aa6c337ee535872a1ec3b0640ea25ff9c580c98ad6f0e621a2d67f5f9975a08d58874ec7225cd947b1f3d85e4bfe148871caae7bde42ca7989316f7ece70023" },
                { "tl", "de3be303a2dee36cf2a0580432175fbc1b8705ee1c85813571543fab8586fc7455ac4cba84f014d2b959efbbd0a41c6b157bf26c6aaff50061d7ee638d3935c7" },
                { "tr", "613642aa225e9e1bb85521d67fa83d279083f70ca62e6f20daf7edb5d3ea7a83e9cc7c818941113371cdb606731ba0bf68c45fe0e9782292883417242174da4f" },
                { "trs", "805429a6cdaf745598f2142daabc86b9bccc66adb4073cd95f270fa2d20f503fb481f86f22406f6b8314e66f2bf6fca3ee703e3066a731112c9a4973978b5206" },
                { "uk", "d6835cc05ac7a1b5a00e7ed5f7c209b97ac179e311d2572c0588d3f732f288d71ef60428bd5977009f1684f81d8d8a76be7c8ee210eab90c774bcfe259eed662" },
                { "ur", "282506160c6cc44e726c5dda809c75cb8dfb3270789d5ae975b07442630f23cfc356a2eb5446e0e9a5e6dd3317b1fe5d84779d7b1dbd7ecfe4fd4b5f3859c18d" },
                { "uz", "68315e1c41f05bc49bf7fa7dfbbe46a912881f685eba838f3cbbadc5ab2835021beb0528be79fce18326c6f69b36122a60a005e889feb02cf3742834e903e1e8" },
                { "vi", "a10a761da603c89fead23dda9a0f1f461d988723630897be11b52440b19c763b5c8e3e1254292ba8597ec98ea4f8906ebc37b07d6e9447f8fb81549525a92423" },
                { "xh", "c0a4445eab8fa23094b0cc81f18f81e131704501fb387e76a891975613cbc40c52d105ff1004fe8ce43b69b7c9743579554d0c27e303863371145ef3211bd809" },
                { "zh-CN", "31ba1726f6ac92163e5564b6bf64eb7eb3ced04c8b8cad3be97a2dd5ce5e429627f0a38f144dc912dc744f277fde5bd7d2406146c9ebab5a224816490d7c83fb" },
                { "zh-TW", "74e498f770068716ba3c6c749837856bcb74dc01aed2689782d83d4d6f42a31fd92e6c6d5de870506d0502213c46c91dc25882606aa58efc53aaefe6e5449358" }
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
